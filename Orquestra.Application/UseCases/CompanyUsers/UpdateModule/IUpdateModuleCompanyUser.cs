using Orquestra.Application.UseCases.Companies.Shared;

namespace Orquestra.Application.UseCases.CompanyUsers.UpdateModule;

public interface IUpdateModuleCompanyUser
{
    Task Execute(Guid userIdAuth, CompanyUserUpdateModuleInput input);
}