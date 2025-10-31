using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Jobs.Base;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Infrastructure.Jobs.Schedules;

public sealed class ScheduleStatusJob(IServiceScopeFactory scopeFactory, ILogger<ScheduleStatusJob> logger) : BackgroundService
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

            // Início;
            await CheckAndUpdateStatus(context);

            // Loop;
            await Task.Delay(TimeSpan.FromHours(LOOP_IN_HOUR), stoppingToken);
        }
    }

    private async Task CheckAndUpdateStatus(Context context)
    {
        DateTime now = GetDate();
        int completedValue = (int)ScheduleStatusEnum.Completed;
        int scheduledValue = (int)ScheduleStatusEnum.Scheduled;

        // Atualiza o schedules para ScheduleStatusEnum.Completed caso já venceram;
        string schedulesSql = @"
        UPDATE ""Schedules""
        SET ""ScheduleStatus"" = {0}     -- {0} = novo status: ScheduleStatusEnum.Completed
        WHERE ""DateEnd"" <= {1}         -- {1} = now
          AND ""ScheduleStatus"" = {2};  -- {2} = status antigo: ScheduleStatusEnum.Scheduled: 
        ";

        int schedulesUpdated = await context.Database.ExecuteSqlRawAsync(
            schedulesSql,
            completedValue,    // {0} = novo status: ScheduleStatusEnum.Completed;
            now,               // {1} = now;
            scheduledValue     // {2} = status antigo: ScheduleStatusEnum.Scheduled: 
        );

        if (schedulesUpdated > 0)
        {
            await JobsBase.CreateLog(context, _logger, description: $"Status dos agendamentos atualizados: {schedulesUpdated}");
        }
    }
}