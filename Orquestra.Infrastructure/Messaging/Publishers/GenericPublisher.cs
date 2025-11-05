using Microsoft.Extensions.Logging;
using Orquestra.Infrastructure.Factory.RabbitMQ;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Orquestra.Infrastructure.Messaging.Publishers;

public class GenericPublisher(IRabbitMQConnection connection, ILogger<GenericPublisher> logger) : IGenericPublisher
{
    private readonly IRabbitMQConnection _connection = connection;
    private readonly ILogger<GenericPublisher> _logger = logger;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task Publish<T>(string queueName, T message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentException.ThrowIfNullOrWhiteSpace(queueName);

        try
        {
            string json = JsonSerializer.Serialize(message, _jsonOptions);
            byte[] body = Encoding.UTF8.GetBytes(json);

            BasicProperties properties = new()
            {
                Persistent = true,
                DeliveryMode = DeliveryModes.Persistent
            };

            await _connection.Channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queueName,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken
            );

            // _logger.LogDebug("Mensagem do tipo {MessageType} publicada com sucesso na fila {Queue}", typeof(T).Name, queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar mensagem do tipo {MessageType} na fila {Queue}", typeof(T).Name, queueName);
            throw;
        }
    }
}