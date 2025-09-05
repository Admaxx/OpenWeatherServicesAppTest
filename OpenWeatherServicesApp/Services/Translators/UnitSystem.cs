
namespace OpenWeatherServicesApp.Services.Translator
{
    public class UnitSystem : IUnitSystem
    {
        public List<string> getUnitSystem()
            =>
            new List<string>()
            {
                "metric",
                "standard",
                "imperial"
            };
    }
}
