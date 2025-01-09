using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Data.SqlClient;

namespace Orquestra.Infrastructure.Factory;

public class ConnectionFactory(IConfiguration configuration) : IConnectionFactory
{
    private readonly IConfiguration _configuration = configuration;

    public string ObterStringConnection()
    {
        string nomeConnectionString = _configuration["SystemSettings:ConnectionStringName"] ?? string.Empty;
        string connectionString = _configuration.GetConnectionString(nomeConnectionString) ?? string.Empty;

        return connectionString;
    }

    public SqlConnection ObterSqlServerConnection()
    {
        return new SqlConnection(ObterStringConnection());
    }

    public MySqlConnection ObterMySqlConnection()
    {
        return new MySqlConnection(ObterStringConnection());
    }
}