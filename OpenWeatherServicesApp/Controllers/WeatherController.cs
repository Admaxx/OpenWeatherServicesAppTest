using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenWeatherServicesApp.DBModels;
using OpenWeatherServicesApp.Models.JSON;
using OpenWeatherServicesApp.Models.JsonOptions;
using OpenWeatherServicesApp.Models.Weather;
using OpenWeatherServicesApp.Services;
using OpenWeatherServicesApp.Services.JSON;
using OpenWeatherServicesApp.Services.Translators;

namespace OpenWeatherServicesApp.Controllers
{
    public class WeatherController(
        WeatherDBContext connString, IOptions<APIKeys> apiKeys, mainClass main,
        IWindDirection windDirection, IGetFromJSON getFromJSON, HttpClient httpClient) : Controller
    {
        mainClass _main = main;
        readonly WeatherDBContext _connString = connString;
        readonly APIKeys _apiKeys = apiKeys.Value;
        readonly IWindDirection _windDirection = windDirection;
        readonly IGetFromJSON _getFromJSON = getFromJSON;
        readonly HttpClient _httpClient = httpClient;

        public IActionResult Weather()
            =>
            View();

        public IActionResult getSystems()
            =>
            Json(_connString.Unitsystems
                .AsNoTracking()
                .Select(item => item.UnitSystemValue.ToLower()));

        public IActionResult getCountries()
            =>
            Json(_connString.Countrieslists
                .AsNoTracking()
                .Select(item => item.CountriesSymbol.ToUpper()));

        public IActionResult getCitiesList(string term,
            int page = 2, int pageSize = 10, string country = "")
        {
            int skip = (page - 1) * pageSize;
            var results = _getFromJSON
                .Search(term, skip, pageSize, country);

            return Json(new
            {
                results = results
                .Select(item => new
                {
                    id = item.Id,
                    text = item.name,
                    lat = item.coord.lat,
                    lon = item.coord.lon
                })
            });
        }
        public async Task<IActionResult> JsonObjectReturn([FromBody] weatherRequestModel model)
        {
            if (model == null) return Json(new { });

            _httpClient.BaseAddress = new Uri(_main.weatherUri);
            var response = await _httpClient.GetAsync(
                $"{_main.apiVersion}" +
                $"{_main.firstParameterLatitude}={model.coord.lat}" +
                $"&{_main.parameterLongitude}={model.coord.lon}" +
                $"&{_main.parameterApiKey}={_apiKeys.WheatherApiKey}" +
                $"&{_main.parametrUnitSystem}={model.unitSystem}");

            response.EnsureSuccessStatusCode();

            var stringResult = await response.Content.ReadAsStringAsync();
            var rawWeather = JsonConvert.DeserializeObject<GetWeather>(stringResult);

            var directions = _windDirection._directions();
            var windFunc = directions.Keys.First(test => test(Convert.ToInt32(rawWeather.wind.deg)));
            string windDesc = directions[windFunc];

            string? weatherDesc = await _connString.Weatherpolishdescriptions
                .AsNoTracking()
                .Where(item => item.EngDesc == rawWeather.weather[0].description)
                .Select(item => item.PolDesc)
                .FirstOrDefaultAsync();

            var symbols = (await _connString.Unitsystemsymbols
                .AsNoTracking()
                .Where(item => item.unitsystem.UnitSystemValue == model.unitSystem.ToLower())
                .ToListAsync())
                .ToDictionary(item => item.Description, item => item.UnitSystemSymbol1);

            return Json(new
            {
                temp = $"{Math.Round(Convert.ToDecimal(rawWeather.main.temp.Replace(".", ",")))}{symbols[nameof(rawWeather.main.temp)]}",
                humidity = $"{rawWeather.main.humidity}{_main.humiditySymbol}",
                windSpeed = $"{Math.Round(Convert.ToDecimal(rawWeather.wind.speed.Replace(".", ",")))}{symbols[nameof(rawWeather.wind.speed)]}",
                windDeg = windDesc,
                description = string.IsNullOrEmpty(weatherDesc) ? rawWeather.weather[0].description : weatherDesc,
                icon = rawWeather.weather[0].icon,
                name = rawWeather.name
            });
        }
    }
}