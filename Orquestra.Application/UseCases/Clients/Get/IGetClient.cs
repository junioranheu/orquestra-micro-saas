using Orquestra.Application.UseCases.Clients.Shared;

namespace Orquestra.Application.UseCases.Clients.Get;

public interface IGetClient
{
    Task<List<ClientOutput>?> Execute();
    Task<ClientOutput?> Execute(Guid clientId);
}