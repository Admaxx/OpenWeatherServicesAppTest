namespace OpenWeatherServicesApp.Services.Translators
{
    public class WindDirection : IWindDirection
    {
        mainClass _main;
        public WindDirection() => _main = new mainClass();
        public Dictionary<Func<int, bool>, string> _directions()
            =>
            _main.windDirectionsDesc;
    }
}
