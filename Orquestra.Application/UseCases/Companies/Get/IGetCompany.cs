using Orquestra.Application.UseCases.Companies.Shared;

namespace Orquestra.Application.UseCases.Companies.Get;

public interface IGetCompany
{
    Task<CompanyOutput?> Execute(Guid companyId);
    Task<List<CompanyOutput>?> Execute();
}