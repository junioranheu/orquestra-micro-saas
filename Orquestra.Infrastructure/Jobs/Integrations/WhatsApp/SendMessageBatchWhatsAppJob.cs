using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Jobs.Base.Handlers;

namespace Orquestra.Infrastructure.Jobs.Integrations.WhatsApp;

public sealed class SendMessageBatchWhatsAppJob(IServiceScopeFactory scopeFactory, ISendMessageBatchWhatsAppHandler handler) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ISendMessageBatchWhatsAppHandler _handler = handler;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            Context context = scope.ServiceProvider.GetRequiredService<Context>();

            // Início;
            await _handler.ExecuteAsync(stoppingToken);

            // Loop;
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}