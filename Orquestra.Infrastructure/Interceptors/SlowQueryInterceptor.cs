using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace Orquestra.Infrastructure.Interceptors;

public class SlowQueryInterceptor(ILoggerFactory loggerFactory) : DbCommandInterceptor
{
    private readonly ILogger _logger = loggerFactory.CreateLogger(nameof(SlowQueryInterceptor));

    public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
    {
        int slowQueryThresholdInMs = GetThreshold(command);

        if (eventData.Duration.TotalMilliseconds > slowQueryThresholdInMs)
        {
            LogLongQuery(command, eventData);
        }

        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        int slowQueryThresholdInMs = GetThreshold(command);

        if (eventData.Duration.TotalMilliseconds > slowQueryThresholdInMs)
        {
            LogLongQuery(command, eventData);
        }

        return base.ReaderExecuted(command, eventData, result);
    }

    #region extras
    private void LogLongQuery(DbCommand command, CommandExecutedEventData eventData)
    {
        _logger.LogWarning($"Long query: {command.CommandText}. Duration: {eventData.Duration.TotalMilliseconds} ms");
    }

    private static int GetThreshold(DbCommand command)
    {
        string sql = command.CommandText.ToUpper();

        if (sql.StartsWith("SELECT"))
        {
            if (sql.Contains("JOIN") || sql.Contains("GROUP BY") || sql.Contains("SUBQUERY"))
            {
                return 300; // Média ou pesada;
            }

            return 100; // Simples;
        }

        if (sql.StartsWith("UPDATE") || sql.StartsWith("DELETE"))
        {
            if (sql.Contains("WHERE"))
            {
                return 200;
            }

            return 500; // Sem WHERE ou update/delete grande;
        }

        return 100; // Default;
    }
    #endregion
}