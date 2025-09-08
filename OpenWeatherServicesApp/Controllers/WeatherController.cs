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
        IWindDirection windDirection, IGetFromJSON getFromJSON, IDaysOfWeek daysOfWeek) : Controller
    {
        mainClass _main = main;
        readonly WeatherDBContext _connString = connString;
        readonly APIKeys _apiKeys = apiKeys.Value;
        readonly IWindDirection _windDirection = windDirection;
        readonly IDaysOfWeek _daysOfWeek = daysOfWeek;
        readonly IGetFromJSON _getFromJSON = getFromJSON;

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

        public IActionResult getOneCiteBySelect(string term, 
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
            if (model != null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_main.weatherUri);
                    var response = await client.GetAsync(
                        $"{_main.apiVersion}" +
                        $"{_main.firstParameterLatitude}={model.coord.lat}" +
                        $"&{_main.parameterLongitude}={model.coord.lon}" +
                        $"&{_main.parameterApiKey}={_apiKeys.WheatherApiKey}" +
                        $"&{_main.parametrUnitSystem}={model.unitSystem}");

                    response.EnsureSuccessStatusCode();

                    var stringResult = await response.Content.ReadAsStringAsync();
                    var rawWeather = JsonConvert.DeserializeObject<GetWeather>(stringResult);

                    string _getWindDesc = new(
                           _windDirection._directions()
                           [
                              _windDirection._directions().Keys.First(test => test(Convert.ToInt32(rawWeather!.wind.deg)))
                           ]
                           );

                    string _getWeatherDesc = await _connString.Weatherpolishdescriptions
                        .AsNoTracking()
                        .Where(item => item.EngDesc == rawWeather!.weather[0].description)
                        .Select(item => item.PolDesc)
                        .FirstAsync();

                    var getSymbols = await _connString.Unitsystemsymbols
                        .AsNoTracking()
                        .Where(item => item.unitsystem.UnitSystemValue == model.unitSystem.ToLower())
                        .Select(item => new { item.Description, item.UnitSystemSymbol1 })
                        .ToListAsync();

                    Console.WriteLine(nameof(rawWeather.wind.speed));

                    var temp = Math.Round(Convert.ToDecimal(rawWeather!.main.temp.Replace(".", ",")));
                    var speed = Math.Round(Convert.ToDecimal(rawWeather.wind.speed.Replace(".", ",")));

                    return Json(new
                        {
                            temp = temp + getSymbols.First(item => item.Description == nameof(rawWeather.main.temp)).UnitSystemSymbol1,
                            humidity = rawWeather.main.humidity + _main.humiditySymbol,
                            windSpeed = speed + getSymbols.First(item => item.Description == nameof(rawWeather.wind.speed)).UnitSystemSymbol1,
                            windDeg = $"{_getWindDesc}",
                            description = _getWeatherDesc == string.Empty ? rawWeather!.weather[0].description : Convert.ToString(_getWeatherDesc),
                            icon = rawWeather.weather[0].icon,
                            name = rawWeather.name,
                            country = rawWeather.sys.country,
                            dayOfWeek = _daysOfWeek.getDayOfWeekPolish().GetValueOrDefault(Convert.ToInt32(DateTime.Now.DayOfWeek)),
                        });
                }
            }
            return 
                Json(new{});
        }
    }
}