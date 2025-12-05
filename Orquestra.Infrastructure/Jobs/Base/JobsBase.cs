using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Infrastructure.Jobs.Base;

public partial class JobsBase()
{
    public static async Task CreateLog(Context context, ILogger logger, string description)
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

    public static TimeOnly ConvertLocalToUtc(TimeOnly localTime)
    {
        // Converte o TimeOnly para um DateTime baseado na data de hoje;
        DateTime localDateTime = DateTime.Today.Add(localTime.ToTimeSpan());

        // Converte para UTC;
        DateTime utcDateTime = localDateTime.ToUniversalTime();

        // Retorna só a parte TimeOnly;
        return TimeOnly.FromDateTime(utcDateTime);
    }

    public static DateTime GetNextRunDateTime(DateTime now, TimeOnly target)
    {
        DateTime todayAtTarget = now.Date.Add(target.ToTimeSpan());

        // Se já passou o horário hoje → agenda pra amanhã;
        DateTime nextRunDateTime = todayAtTarget > now ? todayAtTarget : todayAtTarget.AddDays(1);

        return nextRunDateTime;
    }
}