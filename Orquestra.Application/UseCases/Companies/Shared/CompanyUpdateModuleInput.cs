using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Companies.Shared;

public sealed class CompanyUpdateModuleInput
{
    public required Guid CompanyId { get; set; }
    public ModuleEnum[]? CompanyModules { get; set; }
}