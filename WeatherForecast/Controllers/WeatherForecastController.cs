using Microsoft.AspNetCore.Mvc;
using WeatherForecast.ApplicationServices;

namespace WeatherForecast.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IWeatherDataProviderService _weatherDataProviderService;

    public WeatherForecastController(IWeatherDataProviderService weatherDataProviderService)
    {
        _weatherDataProviderService = weatherDataProviderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetWeather()
    {
        var weatherData = await _weatherDataProviderService.Fetch();
        return Ok(weatherData);
    }
}