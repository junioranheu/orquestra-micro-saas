using Orquestra.Application.UseCases.Companies.Shared;

namespace Orquestra.Application.UseCases.Companies.Update;

public interface IUpdateCompany
{
    Task<CompanyOutput> Execute(Guid userIdAuth, CompanyInput input);
}