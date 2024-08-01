using Dapper;
using System.Data.SqlClient;

namespace WeatherForecast.Data;

public class WeatherDataRepository : IWeatherDataQueryRepository,IWeatherDataCommandRepository
{
    private readonly string _connectionString;

    public WeatherDataRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task AddOrUpdateAsync(string jsonData)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            @"IF EXISTS 
                 (SELECT 1 FROM WeatherData WHERE Id = 1) 
                 UPDATE WeatherData SET JsonData = @JsonData WHERE Id = 1 
                 ELSE 
                 INSERT INTO WeatherData (Id, JsonData) 
                 VALUES (1, @JsonData)", new { JsonData = jsonData });
    }

    public async Task<string?> GetAsync()
    {
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<string>("SELECT JsonData FROM WeatherData WHERE Id = 1");
    }
}