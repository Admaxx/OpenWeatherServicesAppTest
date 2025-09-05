namespace OpenWeatherServicesApp.Services.Translator
{
    public class WeatherDescription : IWeatherDescription
    {
        public Dictionary<string, string> _description()
        {
            return new Dictionary<string, string>()
            { 
                {"mist", "mgła" },

                {"clear sky", "czyste niebo" },
                {"few clouds", "kilka chmur" },

                {"scattered clouds", "rozproszone chmury" },
                {"broken clouds", "rozbite chmury" },
                {"overcast clouds", "zachmurzone niebo" },

                {"shower rain", "rzęsisty deszcz" },
                {"thunderstorm", "burza" },

                {"snow", "śnieg" },
                {"rain", "deszcz" }
            };
        }
    }
}
