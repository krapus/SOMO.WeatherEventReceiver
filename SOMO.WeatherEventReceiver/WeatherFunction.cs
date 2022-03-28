using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using SOMO.WeatherEventReceiver.Models;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace SOMO.WeatherEventReceiver
{
    public static class WeatherFunction
    {
        private static MongoClient MongoClient;

        [FunctionName("WeatherFunction")]
        public static async Task RunAsync([ServiceBusTrigger("weathereventmessages", Connection = "AzureBusConnectionstring")] string myQueueItem,
            ILogger log, ExecutionContext context)
        {
            try
            {
                var configuration = GetConnectionString(context);
                var database = GetMongoClient(@configuration["CosmoConnectionString"]).GetDatabase("somo");

                var collection = database.GetCollection<BsonDocument>("somo-weathers");

                await collection.InsertOneAsync(CreateDocument(JsonConvert.DeserializeObject<WeatherMessage>(myQueueItem)));

                log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
            }
            catch (System.Exception ex)
            {
                log.LogInformation($"Something was wrong: {ex.Data}");
            }
        }

        private static IConfigurationRoot GetConnectionString(ExecutionContext context)
        {
            return new ConfigurationBuilder()
             .SetBasePath(context.FunctionAppDirectory)
             .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
             .AddEnvironmentVariables()
             .Build();

        }

        private static MongoClient GetMongoClient(string connectionString)
        {
            if (MongoClient == null)
            {
                MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString)
                );
                settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
                MongoClient = new MongoClient(settings);
            }
            return MongoClient;
        }

        private static BsonDocument CreateDocument(WeatherMessage message)
        {
            return new BsonDocument
            {
                {"location", message?.Location},
                {"text", message?.Text },
                {"icon", message?.Icon },
                {"code", message?.Code }
            };
        }
    }
}
