using Orquestra.Application.UseCases.ClientsFollowUps.Shared;

namespace Orquestra.Application.UseCases.ClientsFollowUps.Create;

public interface ICreateClientFollowUp
{
    Task Execute(Guid userIdAuth, ClientFollowUpInput input);
}