using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace SOMO.WeatherEventReceiver
{
    public static class WeatherFunction
    {
        [FunctionName("WeatherFunction")]
        public static void Run([ServiceBusTrigger("weathereventmessages", Connection = "")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
