using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.CompanyUsers.Shared;

public sealed class CompanyUserFilterInput
{
    public Guid CompanyId { get; set; }

    public CompanyUserRoleEnum? CompanyUserRole { get; set; }

    public ModuleEnum[]? Modules { get; set; } = [];

    public string? FullName { get; set; } = string.Empty;

    public string? Email { get; set; } = string.Empty;
}