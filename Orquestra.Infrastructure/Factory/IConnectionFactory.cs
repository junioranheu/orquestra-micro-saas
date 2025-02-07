using MySqlConnector;

namespace Orquestra.Infrastructure.Factory;

public interface IConnectionFactory
{
    string GetConnectionString();
    MySqlConnection GetMySqlConnection();
}