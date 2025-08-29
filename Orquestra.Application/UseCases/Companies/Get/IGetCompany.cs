using Orquestra.Application.UseCases.Companies.Shared;

namespace Orquestra.Application.UseCases.Companies.Get;

public interface IGetCompany
{
    Task<CompanyOutput> Execute(Guid userIdAuth, Guid companyId);
    Task<List<CompanyOutput>?> Execute();
    Task<List<CompanyOutput>?> Execute(Guid userId);
}