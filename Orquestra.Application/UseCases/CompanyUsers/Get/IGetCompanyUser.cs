using Orquestra.Domain.Entities;

namespace Orquestra.Application.UseCases.CompanyUsers.Get;

public interface IGetCompanyUser
{
    Task<List<CompanyUser>?> Execute(Guid companyId, Guid userId);
    Task<bool> CheckIfUserIsFromCompany(Guid? companyId, Guid? userId, bool isAdmin, bool throwError = true);
}