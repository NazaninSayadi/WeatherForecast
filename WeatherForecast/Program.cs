using Microsoft.Extensions.Caching.Memory;
using WeatherForecast.Data;
using WeatherForecast.Services.ExternalWeatherService;
using WeatherForecast.Services.WeatherDataProviderService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IWeatherDataProviderService, WeatherDataProviderService>();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<IWeatherDataCommandRepository, WeatherDataRepository>(sp => new WeatherDataRepository(builder.Configuration.GetConnectionString("Command&QueryConnectionString")));
builder.Services.AddScoped<IWeatherDataQueryRepository, WeatherDataRepository>(sp => new WeatherDataRepository(builder.Configuration.GetConnectionString("Command&QueryConnectionString")));
builder.Services.AddSingleton<DatabaseInitializer>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    return new DatabaseInitializer(connectionString);
});

builder.Services.AddHttpClient("WeatherForecast", client =>
    {
        client.BaseAddress = new Uri("https://api.open-meteo.com/v1/forecast?latitude=52.52&longitude=13.41&hourly=temperature_2m,relativehumidity_2m,windspeed_10m");
        client.Timeout = TimeSpan.FromSeconds(3);
    });

var app = builder.Build();

using var scope = app.Services.CreateScope();
var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
databaseInitializer.InitializeDatabase();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();