using Microsoft.Extensions.Configuration;
using Npgsql;
using Orquestra.Infrastructure.Factory.DataBase;

namespace Orquestra.UnitTests.Tests.Factory;

public sealed class ConnectionFactoryTests
{
    private readonly IConfiguration _config;

    public ConnectionFactoryTests()
    {
        Dictionary<string, string> inMemorySettings = new()
        {
            { "SystemSettings:ConnectionStringName", "Default" },
            { "ConnectionStrings:Default", "Server=127.0.0.1;Database=test;User Id=test;Password=123;" }
        };

        _config = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings!).Build();
    }

    [Fact]
    public void GetConnectionString_ShouldReturnConnectionFromAppSettings()
    {
        // Arrange;
        Dictionary<string, string> inMemorySettings = new()
        {
            { "SystemSettings:ConnectionStringName", "Default" },
            { "ConnectionStrings:Default", "Server=127.0.0.1;Database=test;User Id=test;Password=123;" }
        };

        IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings!).Build();
        DataBaseConnection factory = new(config);

        // Act;
        string connStr = factory.GetConnectionString();

        // Assert;
        Assert.Equal("Server=127.0.0.1;Database=test;User Id=test;Password=123;", connStr);
    }

    [Fact]
    public void GetConnectionString_ShouldReturnConnectionFromUserSecrets()
    {
        // Arrange
        Dictionary<string, string> inMemorySettings = new()
        {
            { "SystemSettings:ConnectionStringName", "SecretsConn" },
            { "ConnectionStrings:SecretsConn", "Server=127.0.0.1;Database=secrets;User Id=secret;Password=321;" }
        };

        IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings!).Build();
        DataBaseConnection factory = new(config);

        // Act;
        string connStr = factory.GetConnectionString();

        // Assert;
        Assert.Equal("Server=127.0.0.1;Database=secrets;User Id=secret;Password=321;", connStr);
    }

    [Fact]
    public void GetConnectionString_ShouldThrow_WhenConnectionStringIsMissing()
    {
        // Arrange;
        Dictionary<string, string> inMemorySettings = new()
        {
            { "SystemSettings:ConnectionStringName", "MissingConn" }
        };

        IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings!).Build();
        DataBaseConnection factory = new(config);

        // Act & Assert;
        Assert.Throws<InvalidOperationException>(() => factory.GetConnectionString());
    }

    [Fact]
    public void GetConnection_ShouldReturnNpgsqlConnection()
    {
        // Arrange;
        DataBaseConnection factory = new(_config);

        // Act;
        NpgsqlConnection connection = factory.GetConnection();

        // Assert;
        Assert.NotNull(connection);
        Assert.IsType<NpgsqlConnection>(connection);
        Assert.Equal(_config.GetConnectionString("Default"), connection.ConnectionString);
    }

    [Fact]
    public void GetConnectionTypeName_ShouldReturnNpgsqlConnectionName()
    {
        // Arrange;
        DataBaseConnection factory = new(_config);

        // Act;
        string typeName = factory.GetConnectionTypeName();

        // Assert;
        Assert.Equal(nameof(NpgsqlConnection), typeName);
    }
}