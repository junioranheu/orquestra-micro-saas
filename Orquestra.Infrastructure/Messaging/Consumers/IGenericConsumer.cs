
namespace Orquestra.Infrastructure.Messaging.Consumers
{
    public interface IGenericConsumer
    {
        Task StartAsync(CancellationToken cancellationToken = default);
    }
}