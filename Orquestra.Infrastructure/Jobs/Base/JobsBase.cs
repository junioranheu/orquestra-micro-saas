using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Jobs.Companies;

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
}
