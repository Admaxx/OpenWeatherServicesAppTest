using OpenWeatherServicesApp.Models.Weather;

namespace OpenWeatherServicesApp.Models.JSON
{
    public class weatherRequestModel
    {
        public Coord coord { get; set; }
        public string unitSystem { get; set; }
    }
}
