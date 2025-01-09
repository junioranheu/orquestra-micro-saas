using MySqlConnector;

namespace Orquestra.Infrastructure.Factory;

public interface IConnectionFactory
{
    MySqlConnection ObterMySqlConnection();
    string ObterStringConnection();
}