using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Companies.UpdateModule;

public interface IUpdateModuleCompany
{
    Task Execute(Guid userIdAuth, Guid companyId, ModuleEnum[] modules);
}