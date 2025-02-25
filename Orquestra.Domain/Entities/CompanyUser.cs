using Orquestra.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Orquestra.Domain.Entities;

public sealed class CompanyUser : Audit
{
    [Key]
    public Guid CompanyUserId { get; set; }

    public Guid CompanyId { get; set; }
    [JsonIgnore]
    [ForeignKey(nameof(CompanyId))]
    public Company? Companies { get; set; }

    public Guid UserId { get; set; }
    [JsonIgnore]
    [ForeignKey(nameof(UserId))]
    public User? Users { get; set; }

    public CompanyUserRoleEnum CompanyUserRole { get; set; }
}