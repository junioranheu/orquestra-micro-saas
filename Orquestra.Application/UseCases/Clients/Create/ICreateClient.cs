using Orquestra.Application.UseCases.Clients.Shared;

namespace Orquestra.Application.UseCases.Clients.Create;

public interface ICreateClient
{
    Task Execute(Guid userId, ClientInput input);
}