using Orquestra.Application.UseCases.CompanyUsers.Shared;

namespace Orquestra.Application.UseCases.CompanyUsers.UpdateModule;

public interface IUpdateModuleCompanyUser
{
    Task Execute(Guid userIdAuth, CompanyUserUpdateModuleInput input);
}