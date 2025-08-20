using Orquestra.Application.UseCases.Clients.Shared;

namespace Orquestra.Application.UseCases.Clients.GetAllByCompanyId;

public interface IGetClientByCompanyId
{
    Task<List<ClientOutput>?> Execute(Guid userId, Guid companyId);
}