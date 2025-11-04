using RabbitMQ.Client;

namespace Orquestra.Infrastructure.Factory.RabbitMQ;

public interface IRabbitMQConnection
{
    IChannel Channel { get; }
    void Dispose();
}