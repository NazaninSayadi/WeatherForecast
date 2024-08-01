namespace WeatherForecast.Data;

public interface IWeatherDataQueryRepository
{
    Task<string?> GetAsync();
}