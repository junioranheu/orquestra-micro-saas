using Orquestra.Application.UseCases.Companies.Shared;

namespace Orquestra.Application.UseCases.Companies.Create;

public interface ICreateCompany
{
    Task<CompanyOutput> Execute(Guid userIdAuth, CompanyInput input);
}