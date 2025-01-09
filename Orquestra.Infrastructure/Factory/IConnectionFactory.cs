using MySqlConnector;
using System.Data.SqlClient;

namespace Orquestra.Infrastructure.Factory;

public interface IConnectionFactory
{
    MySqlConnection ObterMySqlConnection();
    SqlConnection ObterSqlServerConnection();
    string ObterStringConnection();
}