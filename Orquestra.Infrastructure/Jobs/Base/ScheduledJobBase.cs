using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Infrastructure.Jobs.Base;

/// <summary>
/// Classe base para jobs agendados com suporte a retry automático.
/// Cada job concreto define os horários (<see cref="MainRunTime"/> e <see cref="RetryRunTime"/>)
/// e implementa a lógica de negócio em <see cref="ExecuteJobAsync"/>.
/// </summary>
public abstract class ScheduledJobBase(IServiceScopeFactory scopeFactory, ILogger logger) : JobsBase(scopeFactory, logger)
{
    /// <summary>Horário principal de execução (hora local).</summary>
    protected abstract TimeOnly MainRunTime { get; }

    /// <summary>Horário de retry em caso de falha (hora local). Retorne <c>null</c> para desabilitar retry.</summary>
    protected abstract TimeOnly? RetryRunTime { get; }

    /// <summary>
    /// Loop principal: aguarda o horário agendado, executa o job e, em caso de falha, tenta o retry.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            DateTime now = GetDate();
            DateTime nextMainRun = GetNextRunDateTime(now, ConvertLocalToUtc(MainRunTime));
            TimeSpan delay = nextMainRun - now;

            await Task.Delay(delay, stoppingToken);

            bool success = await RunWithScope(stoppingToken);

            if (!success && RetryRunTime.HasValue)
            {
                await ExecuteRetryAsync(stoppingToken);
            }
        }
    }

    /// <summary>
    /// Aguarda até o horário de retry e re-executa o job.
    /// </summary>
    private async Task ExecuteRetryAsync(CancellationToken stoppingToken)
    {
        try
        {
            DateTime retryAt = GetNextRunDateTime(GetDate(), ConvertLocalToUtc(RetryRunTime!.Value));
            TimeSpan retryDelay = retryAt - GetDate();

            if (retryDelay > TimeSpan.Zero)
            {
                await Task.Delay(retryDelay, stoppingToken);
            }

            await RunWithScope(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no retry do job {JobName}.", GetType().Name);
        }
    }

    /// <summary>
    /// Converte um <see cref="TimeOnly"/> em hora local para UTC.
    /// </summary>
    public static TimeOnly ConvertLocalToUtc(TimeOnly localTime)
    {
        // Converte o TimeOnly para um DateTime baseado na data de hoje;
        DateTime localDateTime = DateTime.Today.Add(localTime.ToTimeSpan());

        // Converte para UTC;
        DateTime utcDateTime = localDateTime.ToUniversalTime();

        // Retorna só a parte TimeOnly;
        return TimeOnly.FromDateTime(utcDateTime);
    }

    /// <summary>
    /// Calcula o próximo <see cref="DateTime"/> de execução com base no horário-alvo.
    /// Se o horário já passou hoje, agenda para amanhã.
    /// </summary>
    public static DateTime GetNextRunDateTime(DateTime now, TimeOnly target)
    {
        DateTime todayAtTarget = now.Date.Add(target.ToTimeSpan());

        // Se já passou o horário hoje ? agenda pra amanhã;
        DateTime nextRunDateTime = todayAtTarget > now ? todayAtTarget : todayAtTarget.AddDays(1);

        return nextRunDateTime;
    }
}