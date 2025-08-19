using Orquestra.Application.UseCases.Clients.Shared;

namespace Orquestra.Application.UseCases.Clients.Get;

public interface IGetClient
{
    Task<ClientOutput?> Execute(Guid clientId);
}