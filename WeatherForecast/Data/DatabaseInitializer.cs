using System.Data.SqlClient;

namespace WeatherForecast.Data;

public class DatabaseInitializer
{
    private readonly string _connectionString;

    public DatabaseInitializer(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void InitializeDatabase()
    {
        try
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(_connectionString)
            {
                InitialCatalog = "master"
            };

            using var connection = new SqlConnection(connectionStringBuilder.ConnectionString);
            connection.Open();

            var createDatabaseScript = @"IF NOT EXISTS 
                                            (SELECT * FROM sys.databases WHERE name = 'WeatherForecast') 
                                         BEGIN 
                                            CREATE DATABASE WeatherForecast; 
                                         END";
            
            var createTableScript = @"USE WeatherForecast;
                                      IF NOT EXISTS 
                                          (SELECT * FROM sysobjects WHERE name='WeatherData' AND xtype='U')
                                      BEGIN
                                          CREATE TABLE WeatherData (
                                              Id INT IDENTITY(1,1) PRIMARY KEY,
                                              JsonData NVARCHAR(MAX) NOT NULL
                                          );
                                      END";

            using var createDatabaseCommand = new SqlCommand(createDatabaseScript, connection);
            createDatabaseCommand.ExecuteNonQuery();
            
            using var createTableCommand = new SqlCommand(createTableScript, connection);
            createTableCommand.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}