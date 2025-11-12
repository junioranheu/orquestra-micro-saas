using Orquestra.Application.UseCases.ClientsFollowUps.Shared;

namespace Orquestra.Application.UseCases.ClientsFollowUps.Update;

public interface IUpdateClientFollowUp
{
    Task Execute(Guid userIdAuth, ClientFollowUpInput input);
}