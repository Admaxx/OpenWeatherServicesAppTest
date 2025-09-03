
namespace OpenWeatherServicesApp.Services.Translator
{
    public class WindDirection : IWindDirection
    {
        public Dictionary<Func<int, bool>, string> _directions()
        {
            return new Dictionary<Func<int, bool>, string>()
            {
                { item => item == 0, "cisza" },

                { item => item >= 349 && item <= 11, "północny" },
                { item => item >= 12 && item <= 33, "północno-północno-wschodni" },
                { item => item >= 34 && item <= 56, "północno-wschodni" },
                { item => item >= 57 && item <= 78, "wschodnio-północno-wschodni" },

                { item => item >= 79 && item <= 101, "wschodni" },
                { item => item >= 102 && item <= 123, "wschodnio-południowo-wschodni" },
                { item => item >= 124 && item <= 146, "południowo-wschodni" },
                { item => item >= 147 && item <= 168, "południowo-południowo-wschodni" },

                { item => item >= 169 && item <= 191, "południowy" },
                { item => item >= 192 && item <= 213, "południowo-południowo-zachodni" },
                { item => item >= 214 && item <= 236, "południowo-zachodni" },
                { item => item >= 237 && item <= 258, "zachodnio-południowo-zachodni" },

                { item => item >= 259 && item <= 281, "zachodni" },
                { item => item >= 282 && item <= 303, "zachodnio-północno-zachodni" },
                { item => item >= 304 && item <= 326, "północno-zachodni" },
                { item => item >= 327 && item <= 348, "północno-północno-zachodni" }
            };
        }
    }
}
