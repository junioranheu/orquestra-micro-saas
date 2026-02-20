using Orquestra.Application.UseCases.Clients.Shared;

namespace Orquestra.Application.UseCases.Clients.Create;

public interface ICreateClient
{
    Task<Guid> Execute(Guid userIdAuth, ClientInput input);
}