using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenWeatherServicesApp.Services.JSONOptions;
using OpenWeatherServicesApp.Services.Models.Weather;
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

        public WeatherController(
            IOptions<APIKeys> apiKeys, IWeatherDescription weatherDescription, IWindDirection windDirection)
        {
            this._apiKeys = apiKeys.Value;
            this._weatherDescription = weatherDescription;
            this._windDirection = windDirection;
        }

        public async Task<IActionResult> getCity(string cityFormList)
        {
            return await Task.Run(() => 
                RedirectToAction("Weather", new { City = "Leszno" }));
        }
        public async Task<IActionResult> Weather(string City)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://api.openweathermap.org");
                var response = await client.GetAsync($"/data/2.5/weather?q={City}&appid={_apiKeys.WheatherApiKey}&units=metric");
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

                ViewBag.Temp = rawWeather!.main.Temp;

                return View(new GetWeatherInformations
                {
                    Temp = rawWeather!.main.Temp.Replace(".", ","),
                    Humidity = rawWeather.main.Humidity,
                    WindSpeed = $"{rawWeather.wind.speed}",
                    WindDeg = $"{_getWindDesc}",
                    Description = Convert.ToString(_getWeatherDesc),
                    Icon = rawWeather.weather[0].icon,
                    City = City,
                    Country = rawWeather.sys.country
                }); 
            }
        }
    }
}