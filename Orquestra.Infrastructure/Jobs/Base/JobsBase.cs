using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Infrastructure.Jobs.Base;


/// <summary>
/// Classe base comum para todos os jobs.
/// Contém campos compartilhados, execução com escopo de DI e criação de log.
/// </summary>
public abstract class JobsBase(IServiceScopeFactory scopeFactory, ILogger logger) : BackgroundService
{
    protected readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    protected readonly ILogger _logger = logger;

    /// <summary>
    /// Lógica de negócio do job. Recebe um <see cref="Context"/> com escopo já criado.
    /// </summary>
    protected abstract Task ExecuteJobAsync(Context context, CancellationToken ct);

    /// <summary>
    /// Cria um escopo de DI, resolve o <see cref="Context"/> e executa <see cref="ExecuteJobAsync"/>.
    /// </summary>
    protected async Task<bool> RunWithScope(CancellationToken ct)
    {
        try
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            Context context = scope.ServiceProvider.GetRequiredService<Context>();

            await ExecuteJobAsync(context, ct);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao executar o job {JobName}.", GetType().Name);
            return false;
        }
    }

    /// <summary>
    /// Cria um registro de log do tipo Job no banco de dados.
    /// </summary>
    protected static async Task CreateLog(Context context, ILogger logger, string description)
    {
        Log log = new()
        {
            LogType = LogTypeEnum.Job,
            RequestType = "POST",
            Endpoint = string.Empty,
            Parameters = string.Empty,
            Exception = string.Empty,
            Description = description,
            Status = StatusCodes.Status204NoContent,
            UserId = null
        };

        logger.LogInformation("{description}", description);
        await context.AddAsync(log);
        await context.SaveChangesAsync();
    }
}