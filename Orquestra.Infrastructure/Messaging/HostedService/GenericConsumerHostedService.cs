using Microsoft.Extensions.Hosting;
using Orquestra.Infrastructure.Messaging.Consumers;

namespace Orquestra.Infrastructure.Messaging.HostedService;

public class GenericConsumerHostedService<TMessage>(GenericConsumer<TMessage> consumer) : BackgroundService
{
    private readonly GenericConsumer<TMessage> _consumer = consumer;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumer.StartAsync(stoppingToken);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}