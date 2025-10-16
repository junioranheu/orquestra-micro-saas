using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.CompanyUsers.Shared;

public sealed class CompanyUserInput
{
    public Guid CompanyId { get; set; }

    public Guid UserId { get; set; }

    public CompanyUserRoleEnum CompanyUserRole { get; set; }

    public ModuleEnum[]? UserModules { get; set; } = [];
}