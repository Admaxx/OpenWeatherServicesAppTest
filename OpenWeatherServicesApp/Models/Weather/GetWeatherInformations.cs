using OpenWeatherServicesApp.Models;

namespace OpenWeatherServicesApp.Models.Weather
{
    public class GetWeatherInformations
    {
        public string Id { get; set; }
        public string Temp { get; set; }
        public string Humidity { get; set; }
        public string WindSpeed { get; set; }
        public string WindDeg { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Country { get; set; }
    }
}
