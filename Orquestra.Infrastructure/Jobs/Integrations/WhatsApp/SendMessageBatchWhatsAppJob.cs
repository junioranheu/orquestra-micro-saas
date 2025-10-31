using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Jobs.Base;
using Orquestra.Infrastructure.Jobs.Base.Handlers;

namespace Orquestra.Infrastructure.Jobs.Integrations.WhatsApp;

public sealed class SendMessageBatchWhatsAppJob(IServiceScopeFactory scopeFactory, ILogger<SendMessageBatchWhatsAppJob> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger _logger = logger;
    private const int LOOP_IN_HOUR = 1;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            Context context = scope.ServiceProvider.GetRequiredService<Context>();
            ISendMessageBatchWhatsAppHandler handler = scope.ServiceProvider.GetRequiredService<ISendMessageBatchWhatsAppHandler>();

            // Início;
            await SendMessages(context, handler, stoppingToken);

            // Loop;
            await Task.Delay(TimeSpan.FromHours(LOOP_IN_HOUR), stoppingToken);
        }
    }

    private async Task SendMessages(Context context, ISendMessageBatchWhatsAppHandler handler, CancellationToken stoppingToken)
    {
        int messagesSent = await handler.ExecuteAsync(stoppingToken);

        if (messagesSent > 0)
        {
            await JobsBase.CreateLog(context, _logger, description: $"Mensagens enviadas: {messagesSent}");
        }
    }
}