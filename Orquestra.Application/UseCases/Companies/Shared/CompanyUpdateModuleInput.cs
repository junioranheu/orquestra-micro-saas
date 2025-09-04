using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Companies.Shared;

public sealed class CompanyUpdateModuleInput
{
    public Guid CompanyId { get; set; }
    public ModuleEnum[]? Modules { get; set; }
}