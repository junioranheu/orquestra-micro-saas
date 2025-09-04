using Orquestra.Application.UseCases.Companies.Shared;

namespace Orquestra.Application.UseCases.Companies.GetModule;

public interface IGetModuleCompany
{
    Task<CompanyModulesOutput> Execute(Guid userIdAuth, Guid companyId);
}