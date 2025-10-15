namespace Orquestra.Application.UseCases.CompanyUsers.Shared;

public sealed class CompanyUserInviteInput
{
    public Guid CompanyId { get; set; }

    public required string Email { get; set; }
}