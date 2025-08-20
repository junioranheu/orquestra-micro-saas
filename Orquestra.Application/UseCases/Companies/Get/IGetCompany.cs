using Orquestra.Application.UseCases.Companies.Shared;

namespace Orquestra.Application.UseCases.Companies.Get;

public interface IGetCompany
{
    Task<CompanyOutput?> Execute(Guid userId, Guid companyId);
    Task<List<CompanyOutput>?> Execute();
}