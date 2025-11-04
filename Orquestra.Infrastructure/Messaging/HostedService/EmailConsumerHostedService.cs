using Microsoft.Extensions.Hosting;
using Orquestra.Infrastructure.Messaging.Consumers;

namespace Orquestra.Infrastructure.Messaging.HostedService;

public class EmailConsumerHostedService(EmailConsumer consumer) : IHostedService
{
    private readonly EmailConsumer _consumer = consumer;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _consumer.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}