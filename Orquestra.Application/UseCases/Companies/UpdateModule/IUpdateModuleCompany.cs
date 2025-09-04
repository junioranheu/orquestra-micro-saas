using Orquestra.Application.UseCases.Companies.Shared;

namespace Orquestra.Application.UseCases.Companies.UpdateModule;

public interface IUpdateModuleCompany
{
    Task Execute(Guid userIdAuth, CompanyUpdateModuleInput input);
}