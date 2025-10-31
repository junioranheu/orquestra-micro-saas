using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orquestra.Infrastructure.Jobs.Base.Handlers;

namespace Orquestra.Infrastructure.Jobs.Integrations.WhatsApp;

public sealed class SendMessageBatchWhatsAppJob(IServiceScopeFactory scopeFactory) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            ISendMessageBatchWhatsAppHandler handler = scope.ServiceProvider.GetRequiredService<ISendMessageBatchWhatsAppHandler>();

            // Início;
            await handler.ExecuteAsync(stoppingToken);

            // Loop;
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}