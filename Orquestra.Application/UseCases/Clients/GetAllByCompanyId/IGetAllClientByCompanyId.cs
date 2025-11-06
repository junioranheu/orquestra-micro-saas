using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.Shared;

namespace Orquestra.Application.UseCases.Clients.GetAllByCompanyId;

public interface IGetAllClientByCompanyId
{
    Task<List<ClientOutput>?> Execute(Guid userIdAuth, Guid companyId);
    Task<(IEnumerable<ClientOutput> output, int count)> Execute(PaginationInput pagination, ClientInput input, Guid userIdAuth, Guid companyId);
}