using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Jobs.Base;
using Orquestra.Infrastructure.Jobs.Base.Handlers;

namespace Orquestra.Infrastructure.Jobs.Integrations.WhatsApp;

public sealed class SendMessageBatchWhatsAppJob(IServiceScopeFactory scopeFactory, ILogger<SendMessageBatchWhatsAppJob> logger) : IntervalJobBase(scopeFactory, logger)
{
    protected override TimeSpan Interval => TimeSpan.FromMinutes(SystemConsts.Jobs.SendMessageBatchWhatsAppJob_IntervalMinutes);

    protected override async Task ExecuteJobAsync(Context context, CancellationToken ct)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        ISendMessageBatchWhatsAppHandler handler = scope.ServiceProvider.GetRequiredService<ISendMessageBatchWhatsAppHandler>();

        await SendMessages(context, handler, ct);
    }

    #region extras
    private async Task SendMessages(Context context, ISendMessageBatchWhatsAppHandler handler, CancellationToken stoppingToken)
    {
        int messagesSent = await handler.ExecuteAsync(stoppingToken);

        if (messagesSent > 0)
        {
            await CreateLog(context, _logger, description: $"Mensagens enviadas: {messagesSent}");
        }
    }
    #endregion
}