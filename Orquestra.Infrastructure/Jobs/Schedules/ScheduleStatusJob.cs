using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Jobs.Base;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Infrastructure.Jobs.Schedules;

public sealed class ScheduleStatusJob(IServiceScopeFactory scopeFactory, ILogger<ScheduleStatusJob> logger) : IntervalJobBase(scopeFactory, logger)
{
    protected override TimeSpan Interval => TimeSpan.FromMinutes(SystemConsts.Jobs.ScheduleStatusJob_IntervalMinutes);

    protected override async Task ExecuteJobAsync(Context context, CancellationToken ct)
    {
        int schedulesUpdated = await ScheduleStatusUpdater.CheckAndUpdateStatus(context, ct);

        if (schedulesUpdated > 0)
        {
            await CreateLog(context, logger, description: $"Status dos agendamentos atualizados: {schedulesUpdated}");
        }
    }
}

public sealed class ScheduleStatusJob_SPECIFIC_RUN_TIME(IServiceScopeFactory scopeFactory, ILogger<ScheduleStatusJob_SPECIFIC_RUN_TIME> logger) : ScheduledJobBase(scopeFactory, logger)
{
    protected override TimeOnly MainRunTime => new(0, 0);
    protected override TimeOnly? RetryRunTime => new(1, 0);

    protected override async Task ExecuteJobAsync(Context context, CancellationToken ct)
    {
        int schedulesUpdated = await ScheduleStatusUpdater.CheckAndUpdateStatus(context, ct);

        if (schedulesUpdated > 0)
        {
            await CreateLog(context, logger, description: $"Status dos agendamentos atualizados: {schedulesUpdated}");
        }
    }
}

#region extras
internal static class ScheduleStatusUpdater
{
    public static async Task<int> CheckAndUpdateStatus(Context context, CancellationToken ct = default)
    {
        DateTime now = GetDate();

        int completedValue = (int)ScheduleStatusEnum.Completed;
        int scheduledValue = (int)ScheduleStatusEnum.Scheduled;

        string schedulesSql = """
            UPDATE "Schedules"
            SET "ScheduleStatus" = {0}      -- {0} = novo status: ScheduleStatusEnum.Completed
            WHERE "DateEnd" <= {1}          -- {1} = now
              AND "ScheduleStatus" = {2};   -- {2} = status antigo: ScheduleStatusEnum.Scheduled: 
            """;

        int schedulesUpdated = await context.Database.ExecuteSqlRawAsync(
            sql: schedulesSql,
            parameters: [completedValue, now, scheduledValue],
            cancellationToken: ct
        );

        return schedulesUpdated;
    }
}
#endregion