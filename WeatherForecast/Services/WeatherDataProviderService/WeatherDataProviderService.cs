using WeatherForecast.Services.ExternalWeatherService;

namespace WeatherForecast.Services.WeatherDataProviderService;

public class WeatherDataProviderService : IWeatherDataProviderService
{
    private readonly IWeatherService _weatherService;

    public WeatherDataProviderService(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    public async Task<string?> Fetch()
    {
        try
        {
            var weatherData = await _weatherService.FetchWeatherDataAsync();

            //Save to db            
            return weatherData;
        }
        catch (Exception ex)
        {
            //read from db
            return null;
        }
    }
}