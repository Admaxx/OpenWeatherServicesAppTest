namespace OpenWeatherServicesApp.Services.Translator
{
    public interface IWindDirection
    {
        Dictionary<Func<int, bool>, string> _directions();
    }
}