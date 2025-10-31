using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Jobs.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orquestra.Infrastructure.Jobs.Integrations.WhatsApp;

public sealed class SendMessageBatchWhatsAppJob(IServiceScopeFactory scopeFactory) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            Context context = scope.ServiceProvider.GetRequiredService<Context>();

            ISendMessageBatchWhatsApp useCase = scope.ServiceProvider.GetRequiredService<ISendMessageBatchWhatsApp>();

            // Início;
            await CheckAndExpirePlans(context);

            // Loop;
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}