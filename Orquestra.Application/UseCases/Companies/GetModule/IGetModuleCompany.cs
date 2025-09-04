using Orquestra.Application.UseCases.Companies.Shared;

namespace Orquestra.Application.UseCases.Companies.GetModule;

public interface IGetModuleCompany
{
    Task<CompanyModulesOutput> Execute(Guid userId, Guid companyId);
}