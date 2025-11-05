namespace Orquestra.Infrastructure.Messaging.Publishers;

public interface IGenericPublisher
{
    Task Publish<T>(string queueName, T message, CancellationToken cancellationToken = default);
}