using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.CompanyUsers.GetModule;

public interface IGetModuleCompanyUser
{
    Task<(ModuleEnum[] modules, List<string> modulesStr)> Execute(Guid userIdAuth, Guid companyId);
}