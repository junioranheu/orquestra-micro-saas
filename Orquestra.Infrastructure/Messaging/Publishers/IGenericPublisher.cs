namespace Orquestra.Infrastructure.Messaging.Publishers;

public interface IGenericPublisher
{
    Task PublishAsync<T>(string queueName, T message, CancellationToken cancellationToken = default);
    Task PublishToExchangeAsync<T>(string exchange, string routingKey, T message, CancellationToken cancellationToken = default);
}