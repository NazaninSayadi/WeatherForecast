using WeatherForecast.Services.ExternalWeatherService;
using WeatherForecast.Services.WeatherDataProviderService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IWeatherDataProviderService, WeatherDataProviderService>();
builder.Services.AddScoped<IWeatherService, WeatherService>();

builder.Services.AddHttpClient("WeatherForecast", (serviceProvider, client) =>
    {
        client.BaseAddress =
            new Uri(
                "https://api.open-meteo.com/v1/forecast?latitude=52.52&longitude=13.41&hourly=temperature_2m,relativehumidity_2m,windspeed_10m");
        client.Timeout = TimeSpan.FromSeconds(15);
    })
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler()
    {
        PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5)
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();