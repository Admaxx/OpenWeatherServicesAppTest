using OpenWeatherServicesApp.Models.Weather;

namespace CityJSONExample.Models
{
    public class CityDto
    {
        public string Id { get; set; }
        public string name { get; set; }
        public string country { get; set; }

        public Coord coord { get; set; }
    }
}
