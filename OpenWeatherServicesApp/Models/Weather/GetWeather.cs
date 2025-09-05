namespace OpenWeatherServicesApp.Models.Weather
{
    public class GetWeather
    {
        public string name { get; set; }
        public Main main { get; set; }
        public Sys sys { get; set; }
        public Wind wind { get; set; }
        public Weather[] weather { get; set; }
    }
}
