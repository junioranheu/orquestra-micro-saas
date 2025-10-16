using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.CompanyUsers.Shared;

public sealed class CompanyUserUpdateModuleInput
{
    public required Guid CompanyId { get; set; }
    public required Guid UserId { get; set; }
    public ModuleEnum[]? UserModules { get; set; }
}