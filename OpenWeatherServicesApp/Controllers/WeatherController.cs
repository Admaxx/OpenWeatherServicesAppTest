using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenWeatherServicesApp.Models.JsonOptions;
using OpenWeatherServicesApp.Models.Weather;
using OpenWeatherServicesApp.Services.JSON;
using OpenWeatherServicesApp.Services.Translator;
using System.Text;

namespace OpenWeatherServicesApp.Controllers
{
    public class WeatherController : Controller
    {
        StringBuilder? _getWindDesc;
        StringBuilder? _getWeatherDesc;

        private readonly APIKeys _apiKeys;
        private readonly IWeatherDescription _weatherDescription;
        private readonly IWindDirection _windDirection;
        private readonly IGetFromJSON _getFromJSON;
        private readonly IUnitSystem _unitSystem;

        public WeatherController(
            IOptions<APIKeys> apiKeys, IWeatherDescription weatherDescription, IWindDirection windDirection
            , IGetFromJSON getFromJSON, IUnitSystem unitSystem )
        {
            this._apiKeys = apiKeys.Value;
            this._weatherDescription = weatherDescription;
            this._windDirection = windDirection;
            this._getFromJSON = getFromJSON;
            this._unitSystem = unitSystem;
        }
        public IActionResult Weather(string? term, int page = 1, int pageSize = 20)
            =>
            View();
        
        public IActionResult getSystems([FromBody] string term = "")
            =>
            Json(_unitSystem.getUnitSystem());
        
        public IActionResult getOneCiteBySelect(string term, int page = 2, int pageSize = 10)
        {
            int skip = (page - 1) * pageSize;
            var results = _getFromJSON.Search(term, skip, pageSize);
            return Json(new
            {
                results = results.Select(item => new
                {
                    id = item.Id,
                    text = item.name,
                    lat = item.coord.lat,
                    lon = item.coord.lon,
                })
            });
        }
        public async Task<IActionResult> JsonObjectReturn([FromBody] Coord coords)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://api.openweathermap.org");
                var response = await client.GetAsync($"/data/2.5/weather?lat={coords.lat}&lon={coords.lon}&appid={_apiKeys.WheatherApiKey}&units=metric");
                Console.WriteLine(response);
                response.EnsureSuccessStatusCode();

                var stringResult = await response.Content.ReadAsStringAsync();
                var rawWeather = JsonConvert.DeserializeObject<GetWeather>(stringResult);

                _getWindDesc = new(
                       _windDirection._directions()
                       [
                          _windDirection._directions().Keys.First(test => test(Convert.ToInt32(rawWeather.wind.deg)))
                       ]
                       );
                _getWeatherDesc = new(_weatherDescription._description().GetValueOrDefault(rawWeather!.weather[0].description));

                return Json(new
                {
                    temp = rawWeather!.main.temp,
                    humidity = rawWeather.main.humidity,
                    windSpeed = $"{rawWeather.wind.speed}",
                    windDeg = $"{_getWindDesc}",
                    description = Convert.ToString(_getWeatherDesc),
                    icon = rawWeather.weather[0].icon,
                    name = rawWeather.name
                });
            }
        }
    }
}