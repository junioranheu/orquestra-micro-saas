using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.Shared;

namespace Orquestra.Application.UseCases.Clients.GetAllByCompanyId;

public interface IGetClientByCompanyId
{
    Task<List<ClientOutput>?> Execute(Guid userIdAuth, Guid companyId);
    Task<(IEnumerable<ClientOutput> output, int count)> Execute(PaginationInput pagination, Guid userIdAuth, Guid companyId);
}