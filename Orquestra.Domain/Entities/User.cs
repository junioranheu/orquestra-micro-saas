using Microsoft.EntityFrameworkCore;
using Orquestra.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Orquestra.Domain.Entities;

[Index(nameof(Email))]
public sealed class User : Audit
{
    [Key]
    public Guid UserId { get; set; }

    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    public UserRoleEnum Role { get; set; }
}