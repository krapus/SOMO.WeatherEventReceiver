namespace SOMO.WeatherEventReceiver.Models
{
    internal class WeatherMessage
    {
        public string Location { get; set; }
        public string Text { get; set; }
        public string Icon { get; set; }
        public int Code { get; set; }
    }
}
