using Npgsql;

namespace Orquestra.Infrastructure.Factory;

public interface IDataBaseConnection
{
    string GetConnectionString();
    NpgsqlConnection GetConnection();
    string GetConnectionTypeName();
}