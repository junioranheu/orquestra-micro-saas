using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.CompanyUsers.Shared;

public sealed class CompanyUserOutput
{
    public Guid CompanyUserId { get; set; }

    public Guid CompanyId { get; set; }
    public Company? Companies { get; set; }

    public Guid UserId { get; set; }
    public User? Users { get; set; }

    public CompanyUserRoleEnum CompanyUserRole { get; set; }

    public DateTime CreatedDate { get; set; }
}