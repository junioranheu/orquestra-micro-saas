using Orquestra.Application.UseCases.Clients.Shared;

namespace Orquestra.Application.UseCases.Clients.GetByCompanyId;

public interface IGetClientByCompanyId
{
    Task<List<ClientOutput>?> Execute(Guid companyId);
}