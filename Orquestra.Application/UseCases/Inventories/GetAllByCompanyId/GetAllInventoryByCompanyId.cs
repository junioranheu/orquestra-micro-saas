using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Inventories.Shared;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Inventories.GetAllByCompanyId;

public sealed class GetAllInventoryByCompanyId(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IGetAllInventoryByCompanyId
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task<(IEnumerable<Inventory> output, int count)> Execute(PaginationInput pagination, Guid userIdAuth, Guid companyId, InventoryInput input)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: false);

        var query = _context.Inventories.
                    AsNoTracking().
                    Where(x =>
                        x.CompanyId == companyId &&
                        x.Status == true &&
                        (string.IsNullOrEmpty(input.Name) || x.Name.ToLower().Contains(input.Name.ToLower())) &&
                        (string.IsNullOrEmpty(input.Description) || x.Description!.ToLower().Contains(input.Description!.ToLower())) &&
                        (input.Quantity.GetValueOrDefault() == 0 || x.Quantity == input.Quantity)
                    ).OrderBy(x => x.Name);

        (IEnumerable<Inventory> output, int count) = await PagedQuery.Execute(query, pagination);

        return (output, count);
    }
}