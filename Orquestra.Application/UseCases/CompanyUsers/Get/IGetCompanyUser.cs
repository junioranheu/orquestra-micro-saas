using Orquestra.Domain.Entities;

namespace Orquestra.Application.UseCases.CompanyUsers.Get;

public interface IGetCompanyUser
{
    Task<List<CompanyUser>?> Execute(Guid companyId, Guid userId);
}