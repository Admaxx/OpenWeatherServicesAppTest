namespace OpenWeatherServicesApp.Services.Translators
{
    public class DaysOfWeek : IDaysOfWeek
    {
        mainClass _main;
        public DaysOfWeek() => _main = new mainClass();
        public Dictionary<int, string> getDayOfWeekPolish()
            =>
            _main.daysOfWeekPolish;
        
    }
}