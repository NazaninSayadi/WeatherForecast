namespace WeatherForecast.ApplicationServices;

public interface IWeatherDataProviderService
{
    Task<string> Fetch();
}