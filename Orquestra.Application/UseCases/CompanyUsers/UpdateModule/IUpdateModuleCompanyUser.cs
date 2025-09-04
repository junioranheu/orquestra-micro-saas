using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.CompanyUsers.UpdateModule;

public interface IUpdateModuleCompanyUser
{
    Task Execute(Guid userIdAuth, Guid companyId, ModuleEnum[] modules);
}