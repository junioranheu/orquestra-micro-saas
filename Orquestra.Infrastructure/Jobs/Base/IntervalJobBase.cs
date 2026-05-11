using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Orquestra.Infrastructure.Jobs.Base;

/// <summary>
/// Classe base para jobs executados em intervalo fixo.
/// Cada job concreto define o <see cref="Interval"/> e implementa a lógica em <see cref="ExecuteJobAsync"/>.
/// </summary>
public abstract class IntervalJobBase(IServiceScopeFactory scopeFactory, ILogger logger) : JobsBase(scopeFactory, logger)
{
    /// <summary>Intervalo entre cada execução do job.</summary>
    protected abstract TimeSpan Interval { get; }

    /// <summary>
    /// Loop principal: executa o job e aguarda o intervalo configurado.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await RunWithScope(stoppingToken);
            await Task.Delay(Interval, stoppingToken);
        }
    }
}