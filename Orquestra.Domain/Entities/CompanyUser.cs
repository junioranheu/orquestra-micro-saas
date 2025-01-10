using Orquestra.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Domain.Entities;

public sealed class CompanyUser
{
    [Key]
    public Guid CompanyUserId { get; set; }

    public Guid CompanyId { get; set; }
    [JsonIgnore]
    public Company? Companies { get; set; }

    public Guid UserId { get; set; }
    [JsonIgnore]
    public User? Users { get; set; }

    public CompanyUserRoleEnum CompanyUserRole { get; set; }

    public DateTime CreatedDate { get; set; } = GetDate();
}