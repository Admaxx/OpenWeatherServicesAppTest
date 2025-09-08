using CityJSONExample.Models;
using Newtonsoft.Json;

namespace OpenWeatherServicesApp.Services.JSON
{
    public class GetFromJSON : IGetFromJSON
    {
        private readonly string _filePath;
        private readonly mainClass main;

        public GetFromJSON(IWebHostEnvironment env)
        {
            main = new mainClass();
            _filePath = Path.Combine(env.ContentRootPath, main.citiesListPath);
        }

        public IEnumerable<CityDto> Search(string? term, int skip, int take, string country = "")
        {
            using var fs = File.OpenRead(_filePath);
            using var sr = new StreamReader(fs);
            using var reader = new JsonTextReader(sr);
            var serializer = new JsonSerializer();

            int count = 0;

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.StartObject)
                {
                    var city = serializer.Deserialize<CityDto>(reader);

                    if ((string.IsNullOrEmpty(term) ||
                        city!.name.Contains(term, StringComparison.OrdinalIgnoreCase))
                        && city.country == country)
                    {
                        yield return city!;
                    }
                }
            }
        }
    }
}