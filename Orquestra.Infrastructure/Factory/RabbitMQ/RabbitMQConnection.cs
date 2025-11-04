using RabbitMQ.Client;

namespace Orquestra.Infrastructure.Factory.RabbitMQ;

public sealed class RabbitMQConnection : IDisposable, IRabbitMQConnection
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private bool _disposed;

    public RabbitMQConnection(string? rabbitMqUrl)
    {
        if (string.IsNullOrWhiteSpace(rabbitMqUrl))
        {
            throw new ArgumentException("RabbitMQ URL não configurada.", nameof(rabbitMqUrl));
        }

        ConnectionFactory factory = new()
        {
            Uri = new Uri(rabbitMqUrl),
            AutomaticRecoveryEnabled = true
        };

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
    }

    public IChannel Channel => _channel;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _channel?.Dispose();
        _connection?.Dispose();

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}