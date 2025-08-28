using Microsoft.Extensions.Configuration;
using Orquestra.Infrastructure.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orquestra.UnitTests.Tests.Factory;

public sealed class ConnectionFactoryTests
{
    private readonly IConfiguration _configuration;

    public ConnectionFactoryTests()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // precisa apontar pra pasta onde tá o appsettings.json
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    [Fact]
    public void Should_GetConnectionString_FromAppSettings()
    {
        // Arrange
        var factory = new ConnectionFactory(_configuration);

        // Act
        var connectionString = factory.GetConnectionString();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(connectionString));
    }

    [Fact]
    public void Should_CreateRealConnection()
    {
        // Arrange
        var factory = new ConnectionFactory(_configuration);

        // Act
        using var conn = factory.GetConnection();
        conn.Open();

        // Assert
        Assert.Equal("NpgsqlConnection", factory.GetConnectionTypeName());
        Assert.Equal(System.Data.ConnectionState.Open, conn.State);
    }
}