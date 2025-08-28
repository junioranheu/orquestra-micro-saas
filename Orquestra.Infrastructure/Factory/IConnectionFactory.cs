using Npgsql;

namespace Orquestra.Infrastructure.Factory;

public interface IConnectionFactory
{
    string GetConnectionString();
    NpgsqlConnection GetConnection();
    string GetConnectionTypeName();
}