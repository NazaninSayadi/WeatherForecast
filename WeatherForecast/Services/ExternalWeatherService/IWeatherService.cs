namespace WeatherForecast.Services.ExternalWeatherService;

public interface IWeatherService
{
    Task<string> FetchWeatherDataAsync();
}