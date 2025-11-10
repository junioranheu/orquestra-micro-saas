using Orquestra.Application.UseCases.Inventories.Shared;
using Orquestra.Application.UseCases.Shared;

namespace Orquestra.Application.UseCases.Inventories.GetAllByCompanyId;

public interface IGetAllInventoryByCompanyId
{
    Task<(IEnumerable<InventoryOutput> output, int count)> Execute(PaginationInput pagination, Guid userIdAuth, Guid companyId, InventoryInput input);
}