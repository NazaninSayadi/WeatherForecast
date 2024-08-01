namespace WeatherForecast.ApplicationServices;

public class WeatherDataProviderService :IWeatherDataProviderService
{
    public WeatherDataProviderService() { }
    public Task<string> Fetch()
    {
        throw new NotImplementedException();
    }
}