using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Enums;
using System.Text.Json.Serialization;

namespace Orquestra.Application.UseCases.CompanyUsers.Shared;

public sealed class CompanyUserOutput
{
    public Guid CompanyUserId { get; set; }

    public Guid CompanyId { get; set; }
    [JsonIgnore]
    public CompanyOutput? Company { get; set; }

    public Guid UserId { get; set; }
    public UserOutput? User { get; set; }

    public CompanyUserRoleEnum CompanyUserRole { get; set; }

    public bool IsAccountVerified { get; set; } = false;

    public bool IsCurrentMainCompanyUser { get; set; } = false;

    public DateTime CreatedDate { get; set; }
}