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
            throw new InvalidOperationException("Erro interno: a connection string está nula.");
        }

        return connectionString;
    }

    public NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(GetConnectionString());
    }

    public string GetConnectionTypeName()
    {
        return nameof(NpgsqlConnection);
    }
}