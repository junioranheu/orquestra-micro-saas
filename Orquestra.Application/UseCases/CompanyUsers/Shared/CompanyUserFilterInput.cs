namespace Orquestra.Application.UseCases.CompanyUsers.Shared;

public sealed class CompanyUserFilterInput
{
    public Guid CompanyId { get; set; }

    public string? CompanyUserRole { get; set; } // Posteriormente convertido a CompanyUserRoleEnum;

    public string? UserModules { get; set; } = string.Empty; // Posteriormente convertido a ModuleEnum;

    public string? FullName { get; set; } = string.Empty;

    public string? Email { get; set; } = string.Empty;
}