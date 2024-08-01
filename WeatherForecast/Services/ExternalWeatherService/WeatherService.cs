namespace WeatherForecast.Services.ExternalWeatherService;

public class WeatherService : IWeatherService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WeatherService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> FetchWeatherDataAsync()
    {
        var client = _httpClientFactory.CreateClient("WeatherForecast");
        var response = await client.GetAsync(client.BaseAddress, CancellationToken.None);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}