using Microsoft.EntityFrameworkCore;

namespace Orquestra.Application.UseCases.Shared;

public class PagedQuery
{
    public static async Task<(IEnumerable<T> linq, int count)> Execute<T>(IQueryable<T> query, PaginationInput pagination, bool isCount = true)
    {
        int count = isCount ? await query.CountAsync() : 0;

        IEnumerable<T> linq = await query.
                              Skip(pagination.IsSelectAll ? 0 : pagination.Index * pagination.Limit).
                              Take(pagination.IsSelectAll ? int.MaxValue : pagination.Limit).
                              ToListAsync();

        return (linq, count);
    }

    public static (IEnumerable<T> linq, int count) Execute<T>(List<T> list, PaginationInput pagination, bool isCount = true)
    {
        int count = isCount ? list.Count : 0;

        IEnumerable<T> linq = list.
                              Skip(pagination.IsSelectAll ? 0 : pagination.Index * pagination.Limit).
                              Take(pagination.IsSelectAll ? int.MaxValue : pagination.Limit).
                              ToList();

        return (linq, count);
    }
}