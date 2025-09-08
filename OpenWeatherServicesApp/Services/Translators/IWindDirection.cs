namespace OpenWeatherServicesApp.Services.Translators
{
    public interface IWindDirection
    {
        public Dictionary<Func<int, bool>, string> _directions();
    }
}