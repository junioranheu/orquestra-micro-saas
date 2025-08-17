using Orquestra.Application.UseCases.CompanyUsers.Shared;

namespace Orquestra.Application.UseCases.CompanyUsers.Get;

public interface IGetCompanyUser
{
    Task<List<CompanyUserOutput>?> Execute(Guid companyId, Guid? userId = null);
}