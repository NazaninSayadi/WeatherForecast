using WeatherForecast.Data;
using WeatherForecast.Services.ExternalWeatherService;
using WeatherForecast.Services.WeatherDataProviderService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IWeatherDataProviderService, WeatherDataProviderService>();
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<IWeatherDataCommandRepository, WeatherDataRepository>(sp => new WeatherDataRepository(""));
builder.Services.AddScoped<IWeatherDataQueryRepository, WeatherDataRepository>(sp => new WeatherDataRepository(""));

builder.Services.AddHttpClient("WeatherForecast", client =>
    {
        client.BaseAddress = new Uri("https://api.open-meteo.com/v1/forecast?latitude=52.52&longitude=13.41&hourly=temperature_2m,relativehumidity_2m,windspeed_10m");
        client.Timeout = TimeSpan.FromSeconds(3);
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