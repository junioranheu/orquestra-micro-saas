using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Orquestra.Infrastructure.Factory;

public class ConnectionFactory(IConfiguration configuration) : IConnectionFactory
{
    private readonly IConfiguration _configuration = configuration;

    public string GetConnectionString()
    {
        string connectionStringName = _configuration["SystemSettings:ConnectionStringName"] ?? string.Empty;
        string connectionString = _configuration.GetConnectionString(connectionStringName) ?? string.Empty;

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new Exception("A connection string está nula");
        }

        return connectionString;
    }

    public NpgsqlConnection GetMySqlConnection()
    {
        return new NpgsqlConnection(GetConnectionString());
    }
}