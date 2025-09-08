using CityJSONExample.Models;
using OpenWeatherServicesApp.Models.Weather;

namespace OpenWeatherServicesApp.Services.JSON
{
    public interface IGetFromJSON
    {
        IEnumerable<CityDto> Search(string term, int page, int pageSize, string country = "");
    }
}