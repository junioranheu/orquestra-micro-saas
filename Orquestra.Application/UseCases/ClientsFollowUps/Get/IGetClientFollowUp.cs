using Orquestra.Application.UseCases.ClientsFollowUps.Shared;

namespace Orquestra.Application.UseCases.ClientsFollowUps.Get;

public interface IGetClientFollowUp
{
    Task<(IEnumerable<ClientFollowUpOutput> output, int count)> Execute(Guid userIdAuth, ClientFollowUpInput input);
}