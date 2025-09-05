using CityJSONExample.Models;
using Newtonsoft.Json;
using OpenWeatherServicesApp.Models.Weather;


namespace OpenWeatherServicesApp.Services.JSON
{
    public class GetFromJSON : IGetFromJSON
    {
        private readonly string _filePath;

        public GetFromJSON(IWebHostEnvironment env)
        {
            _filePath = Path.Combine(env.ContentRootPath, "App_Data", "city.list.json");
        }

        public IEnumerable<CityDto> Search(string? term, int skip, int take)
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
                        && city.country == "PL")
                    {
                        if (count++ < skip) continue;
                        yield return city!;
                        if (--take == 0) yield break;
                    }
                }
            }
        }
    }
}
