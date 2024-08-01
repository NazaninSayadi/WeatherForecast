using Microsoft.Extensions.Caching.Memory;
using WeatherForecast.Data;
using WeatherForecast.Services.ExternalWeatherService;

namespace WeatherForecast.Services.WeatherDataProviderService;

public class WeatherDataProviderService : IWeatherDataProviderService
{
    private readonly IWeatherService _weatherService;
    private readonly IWeatherDataCommandRepository _weatherDataCommandRepository;
    private readonly IWeatherDataQueryRepository _weatherDataQueryRepository;
    private readonly IMemoryCache _cache;

    public WeatherDataProviderService(IWeatherService weatherService,
                                      IWeatherDataCommandRepository weatherDataCommandRepository,
                                      IWeatherDataQueryRepository weatherDataQueryRepository,
                                      IMemoryCache cache)
    {
        _weatherService = weatherService;
        _weatherDataCommandRepository = weatherDataCommandRepository;
        _weatherDataQueryRepository = weatherDataQueryRepository;
        _cache = cache;
    }

    public async Task<string?> Fetch()
    {
        const string cacheKey = "WeatherData";
        try
        {
            var weatherData = await _weatherService.FetchWeatherDataAsync();

            _ =  Task.Run(() =>
            {
                _weatherDataCommandRepository.AddOrUpdateAsync(weatherData);
                _cache.Set(cacheKey, weatherData, TimeSpan.FromMinutes(10));
            });

            return weatherData;
        }
        catch (Exception ex)
        {
            try
            {
                if (_cache.TryGetValue(cacheKey, out string? cachedData))
                {
                    return cachedData;
                }

                if (await _weatherDataQueryRepository.GetAsync() is { } dbData)
                {
                    return dbData;
                }
            }
            catch (Exception e)
            {
                return null;
            }
           
            return null;
        }
    }
}