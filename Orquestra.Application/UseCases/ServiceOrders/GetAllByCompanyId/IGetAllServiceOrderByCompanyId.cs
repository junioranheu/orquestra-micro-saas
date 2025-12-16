using Orquestra.Application.UseCases.ServiceOrders.Shared;
using Orquestra.Application.UseCases.Shared;

namespace Orquestra.Application.UseCases.ServiceOrders.GetAllByCompanyId;

public interface IGetAllServiceOrderByCompanyId
{
    Task<(IEnumerable<ServiceOrderOutput> output, int count)> Execute(PaginationInput pagination, ServiceOrderInput input, Guid userIdAuth, Guid companyId);
}