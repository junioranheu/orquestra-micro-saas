using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.CompanyUsers.GetCurrentMain;

public interface IGetCurrentMainCompanyUser
{
    Task<(CompanyOutput? currentMainCompany, bool isUserAdm)> GetCurrentMainCompany(Guid userId);
    Task<(ModuleEnum[] modules, List<string> modulesStr)> GetCurrentModules(Guid userId);
}