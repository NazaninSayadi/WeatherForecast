namespace WeatherForecast.Data;

public interface IWeatherDataCommandRepository
{
    Task AddOrUpdateAsync(string jsonData);
}