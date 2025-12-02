using Orquestra.Application.UseCases.Clients.Shared;

namespace Orquestra.Application.UseCases.Clients.GetFilter;

public interface IGetFilterClient
{
    Task<ClientFilterOutput?> Execute(Guid userIdAuth, Guid companyId);
}