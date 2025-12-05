using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Jobs.Base;
using Orquestra.Infrastructure.Services.GenericCache;
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
            scheduledValue     // {2} = status antigo: ScheduleStatusEnum.Scheduled;
        );

        if (schedulesUpdated > 0)
        {
            await JobsBase.CreateLog(context, _logger, description: $"Status dos agendamentos atualizados: {schedulesUpdated}");
        }
    }
}

public sealed class ScheduleStatusJob_SPECIFIC_RUN_TIME(IServiceScopeFactory scopeFactory, IGenericCacheService cache, ILogger<ScheduleStatusJob_SPECIFIC_RUN_TIME> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IGenericCacheService _cache = cache;
    private readonly ILogger _logger = logger;

    private static readonly TimeOnly MAIN_RUN_TIME = new(0, 0); // Meia-noite;
    private static readonly TimeOnly RETRY_RUN_TIME = new(1, 0); // 1h da manhã;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            DateTime now = GetDate();

            // Próximo horário principal;
            DateTime nextMainRun = JobsBase.GetNextRunDateTime(now, JobsBase.ConvertLocalToUtc(MAIN_RUN_TIME));

            TimeSpan delay = nextMainRun - now;
            await Task.Delay(delay, stoppingToken);

            bool success = await RunJob( stoppingToken);

            if (!success)
            {
                try
                {
                    DateTime retryAt = JobsBase.GetNextRunDateTime(now, JobsBase.ConvertLocalToUtc(RETRY_RUN_TIME));
                    TimeSpan retryDelay = retryAt - GetDate();

                    if (retryDelay > TimeSpan.Zero)
                    {
                        await Task.Delay(retryDelay, stoppingToken);
                    }

                    await RunJob( stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao re-executar (!success) o job.");
                }
            }
        }
    }

    #region extras
    private async Task<bool> RunJob( CancellationToken ct)
    {
        try
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            Context context = scope.ServiceProvider.GetRequiredService<Context>();

            await CheckAndUpdateStatus(context, ct);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao executar o job.");
            return false;
        }
    }

    private async Task CheckAndUpdateStatus(Context context, CancellationToken ct)
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
            sql: schedulesSql,
            parameters: [completedValue, now, scheduledValue],
            cancellationToken: ct
        );

        if (schedulesUpdated > 0)
        {
            await JobsBase.CreateLog(context, _logger, description: $"Status dos agendamentos atualizados: {schedulesUpdated}");
        }
    }
    #endregion
}