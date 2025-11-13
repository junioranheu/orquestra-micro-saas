using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orquestra.Infrastructure.Messaging.Consumers;

namespace Orquestra.Infrastructure.Messaging.HostedService;

public class GenericConsumerHostedService<TMessage>(GenericConsumer<TMessage> consumer, ILogger<GenericConsumerHostedService<TMessage>> logger) : BackgroundService
{
    private readonly GenericConsumer<TMessage> _consumer = consumer;
    private readonly ILogger<GenericConsumerHostedService<TMessage>> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // _logger.LogInformation("Iniciando GenericConsumerHostedService para mensagens do tipo {MessageType}.", typeof(TMessage).Name);

        try
        {
            await _consumer.StartAsync(stoppingToken);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("GenericConsumerHostedService para {MessageType} sendo encerrado graciosamente.", typeof(TMessage).Name);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Erro fatal no GenericConsumerHostedService para {MessageType}.", typeof(TMessage).Name);
        }
        finally
        {
            await _consumer.DisposeAsync(); 
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        // _logger.LogInformation("Parando GenericConsumerHostedService para {MessageType}.", typeof(TMessage).Name);
        await _consumer.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}