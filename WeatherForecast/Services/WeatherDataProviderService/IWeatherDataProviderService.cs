namespace WeatherForecast.Services.WeatherDataProviderService;

public interface IWeatherDataProviderService
{
    Task<string?> Fetch();
}