namespace WeatherForecastTest;

public class WeatherForecast
{
    private readonly Mock<IWeatherDataQueryRepository> _mockWeatherDataQueryRepository;
    private readonly MemoryCache _memoryCache;
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly WeatherDataProviderService _weatherDataProviderService;
    
    public WeatherForecast()
    {
        Mock<IWeatherDataCommandRepository> mockWeatherDataCommandRepository = new();
        _mockWeatherDataQueryRepository = new(); 
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _mockHttpClientFactory = new (); 
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        WeatherService weatherService = new(_mockHttpClientFactory.Object);
        _weatherDataProviderService = new WeatherDataProviderService(weatherService, mockWeatherDataCommandRepository.Object, _mockWeatherDataQueryRepository.Object, _memoryCache);
    }
    
    [Fact]
    public async Task GetWeather_ShouldReturnWeatherDataWithinFiveSeconds_WhenExternalServiceIsAvailable()
    {
        // Arrange
        SetupExternalServiceToResponseSuccessfully();
        var controller = new WeatherForecastController(_weatherDataProviderService);

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = await controller.GetWeather();
        stopwatch.Stop();

        // Assert
        Assert.IsType<OkObjectResult>(result);
        Assert.True(stopwatch.ElapsedMilliseconds <= 5000);
    }

    [Fact]
    public async Task GetWeather_ShouldReturnNullWithinFiveSeconds_WhenNoDataAvailable()
    {
        // Arrange
        SetupExternalServiceToReturnServiceUnavailable();
        SetupQueryRepositoryToReturnNull();
        var controller = new WeatherForecastController(_weatherDataProviderService);

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = await controller.GetWeather();
        stopwatch.Stop();

        // Assert
        Assert.True(stopwatch.ElapsedMilliseconds <= 5000);
        Assert.Null(((OkObjectResult) result).Value);
    }
    
    [Fact]
    public async Task GetWeather_ShouldReturnCachedData_WhenExternalServiceFails()
    {
        // Arrange
        SetupExternalServiceToReturnTimeout();
        _memoryCache.Set("WeatherData", "{\"temperature\": 20}");
 
        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = await _weatherDataProviderService.Fetch();
        stopwatch.Stop();
 
        // Assert
        Assert.NotNull(result);
        Assert.Equal("{\"temperature\": 20}", result);
        Assert.True(stopwatch.ElapsedMilliseconds <= 5000);    }

    [Fact]
    public async Task GetWeather_ShouldReturnDatabaseData_WhenCacheAndExternalServiceFail()
    {
        // Arrange
        SetupExternalServiceToReturnTimeout();
        SetupQueryRepositoryToReturnValidData();
        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = await _weatherDataProviderService.Fetch();
        stopwatch.Stop();
 
        // Assert
        Assert.NotNull(result);
        Assert.Equal("{\"temperature\": 20}", result);
        Assert.True(stopwatch.ElapsedMilliseconds <= 5000);
        
    }
    
    
    private void SetupQueryRepositoryToReturnValidData()
    {
        _mockWeatherDataQueryRepository.Setup(repo => repo.GetAsync())
            .ReturnsAsync("{\"temperature\": 20}");
    }
    private void SetupQueryRepositoryToReturnNull()
    {
        _mockWeatherDataQueryRepository.Setup(repo => repo.GetAsync())
            .ReturnsAsync((string?) null);
    }
    private void SetupExternalServiceToReturnServiceUnavailable()
    {
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.ServiceUnavailable
            });
        var client = new HttpClient(_mockHttpMessageHandler.Object) {BaseAddress = new Uri("http://test.com")};
        _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
    }
    private void SetupExternalServiceToReturnTimeout()
    {
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException());

        var client = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://test.com")
        };
        _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>()))
            .Returns(client);
    }
    private void SetupExternalServiceToResponseSuccessfully()
    {
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"temperature\": 25}")
            });
        var client = new HttpClient(_mockHttpMessageHandler.Object) {BaseAddress = new Uri("http://test.com")};
        _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
    }
}