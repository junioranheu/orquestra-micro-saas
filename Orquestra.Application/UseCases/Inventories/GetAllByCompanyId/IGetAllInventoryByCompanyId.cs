using Orquestra.Application.UseCases.Inventories.Shared;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;

namespace Orquestra.Application.UseCases.Inventories.GetAllByCompanyId;

public interface IGetAllInventoryByCompanyId
{
    Task<(IEnumerable<Inventory> output, int count)> Execute(PaginationInput pagination, Guid userIdAuth, Guid companyId, InventoryInput input);
}