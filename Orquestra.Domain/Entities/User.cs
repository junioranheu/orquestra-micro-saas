using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Domain.Entities;

[Index(nameof(Email))]
public sealed class User
{
    [Key]
    public Guid UserId { get; set; }

    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    public string? ChangePasswordCode { get; set; } = null;

    public DateTime? ChangePasswordCodeValidity { get; set; }

    public bool Status { get; set; } = true;

    public DateTime Date { get; set; } = GetDate();

    public IEnumerable<UserRole>? UserRoles { get; init; }
}