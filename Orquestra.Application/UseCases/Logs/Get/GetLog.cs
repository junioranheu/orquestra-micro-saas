using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Logs.Get;

public sealed class GetLog(Context context) : IGetLog
{
    private readonly Context _context = context;

    public async Task<(IEnumerable<Log> output, int count)> Execute(PaginationInput pagination, Guid? userId)
    {
        List<string> excludedEndpoints = ["RefreshToken"];

        var query = _context.Logs.
                    Include(x => x.User).
                    AsNoTracking().
                    Where(x =>
                       ((userId == null || userId == Guid.Empty) || x.User!.UserId == userId) &&
                       !excludedEndpoints.Contains(x.Endpoint!)
                    ).OrderByDescending(x => x.CreatedDate);

        (IEnumerable<Log> output, int count) = await PagedQuery.Execute(query, pagination);

        Parallel.ForEach(output, log =>
        {
            if (log.User is not null)
            {
                log.User.Password = string.Empty;
                log.User.RecoverPasswordAnswer = string.Empty;
            }

            log.ChangedFields = GetChangedFieldsFromBeforeAndAfter(log.Parameters);
        });

        return (output, count);
    }
}
