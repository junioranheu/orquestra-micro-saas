using Orquestra.Application.UseCases.Clients.Shared;

namespace Orquestra.Application.UseCases.Clients.Update;

public interface IUpdateClient
{
    Task<ClientOutput> Execute(Guid userId, ClientInput input);
}