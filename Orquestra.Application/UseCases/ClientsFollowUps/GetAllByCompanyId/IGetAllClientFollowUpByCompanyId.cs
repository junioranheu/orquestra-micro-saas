using Orquestra.Application.UseCases.ClientsFollowUps.Shared;
using Orquestra.Application.UseCases.Shared;

namespace Orquestra.Application.UseCases.ClientsFollowUps.GetAllByCompanyId;

public interface IGetAllClientFollowUpByCompanyId
{
    Task<(IEnumerable<ClientFollowUpOutput> output, int count)> Execute(PaginationInput pagination, ClientFollowUpInput input, Guid userIdAuth, Guid companyId);
}