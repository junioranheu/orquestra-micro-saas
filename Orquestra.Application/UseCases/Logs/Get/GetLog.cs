using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Logs.Get;

public sealed class GetLog(Context context) : IGetLog
{
    private readonly Context _context = context;

    public async Task<(IEnumerable<Log> linq, int count)> Execute(PaginationInput pagination, Guid? userId)
    {
        var query = _context.Logs.
                    Include(x => x.Users).
                    AsNoTracking().
                    Where(x =>
                       ((userId == null || userId == Guid.Empty) || x.Users!.UserId == userId)
                    ).
                    OrderByDescending(x => x.CreatedDate);

        (IEnumerable<Log> linq, int count) = await PagedQuery.Execute(query, pagination);

        return (linq, count);
    }
}