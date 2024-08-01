namespace WeatherForecast.Services.WeatherDataProviderService;
public class WeatherDataProviderService :IWeatherDataProviderService
{
    public WeatherDataProviderService() { }
    public Task<string> Fetch()
    {
        throw new NotImplementedException();
    }
}