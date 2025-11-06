using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Inventories.GetAllByCompanyId;

public sealed class GetInventoryByCompanyId(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IGetInventoryByCompanyId
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task<(IEnumerable<Inventory> output, int count)> Execute(PaginationInput pagination, Guid userIdAuth, Guid companyId)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: false);

        var query = _context.Inventories.
                    AsNoTracking().
                    Where(x => x.CompanyId == companyId && x.Status == true).
                    OrderBy(x => x.Name);

        (IEnumerable<Inventory> output, int count) = await PagedQuery.Execute(query, pagination);

        return (output, count);
    }
}