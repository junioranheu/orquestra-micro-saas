using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Companies.Shared;

public sealed class CompanyUserUpdateModuleInput
{
    public Guid CompanyId { get; set; }
    public ModuleEnum[]? Modules { get; set; }
}