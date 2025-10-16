using Microsoft.EntityFrameworkCore;
using Orquestra.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Orquestra.Domain.Entities;

[Index(nameof(CompanyId), nameof(UserId))]
public sealed class CompanyUser : Audit
{
    [Key]
    public Guid CompanyUserId { get; set; }

    public Guid CompanyId { get; set; }
    [JsonIgnore]
    [ForeignKey(nameof(CompanyId))]
    public Company? Company { get; set; }

    public Guid UserId { get; set; }
    [JsonIgnore]
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    public CompanyUserRoleEnum CompanyUserRole { get; set; }

    public ModuleEnum[]? UserModules { get; set; } = [];

    public bool IsCurrentMainCompanyUser { get; set; } = false;

    public Guid? InviterUserId { get; set; }
    [ForeignKey(nameof(InviterUserId))]
    public User? InviterUser { get; init; }
}