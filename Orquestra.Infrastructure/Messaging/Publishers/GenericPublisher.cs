using Orquestra.Infrastructure.Factory.RabbitMQ;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Orquestra.Infrastructure.Messaging.Publishers;

public class GenericPublisher(IRabbitMQConnection connection) : IGenericPublisher
{
    private readonly IRabbitMQConnection _connection = connection;

    public async Task PublishAsync<T>(string queueName, T message, CancellationToken cancellationToken = default)
    {
        string json = JsonSerializer.Serialize(message);
        byte[] body = Encoding.UTF8.GetBytes(json);

        await _connection.Channel.BasicPublishAsync(
            exchange: "",
            routingKey: queueName,
            body: body,
            cancellationToken: cancellationToken
        );
    }

    public async Task PublishToExchangeAsync<T>(string exchange, string routingKey, T message, CancellationToken cancellationToken = default)
    {
        string json = JsonSerializer.Serialize(message);
        byte[] body = Encoding.UTF8.GetBytes(json);

        await _connection.Channel.BasicPublishAsync(
            exchange: exchange,
            routingKey: routingKey,
            body: body,
            cancellationToken: cancellationToken
        );
    }
}