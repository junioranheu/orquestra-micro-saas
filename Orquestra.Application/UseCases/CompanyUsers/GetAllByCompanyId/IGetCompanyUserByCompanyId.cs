using Orquestra.Application.UseCases.CompanyUsers.Shared;

namespace Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;

public interface IGetCompanyUserByCompanyId
{
    Task<List<CompanyUserOutput>?> Execute(Guid companyId, Guid? userId = null);
}