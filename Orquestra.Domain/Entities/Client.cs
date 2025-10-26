using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orquestra.Domain.Entities;

[Index(nameof(CompanyId), nameof(Email), IsUnique = false)]
[Index(nameof(CompanyId), nameof(CPF), IsUnique = false)]
public sealed class Client : Audit
{
    [Key]
    public Guid ClientId { get; set; }

    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Email { get; set; } = string.Empty;

    [MaxLength(14)]
    public string CPF { get; set; } = string.Empty;

    #region location
    [MaxLength(255)]
    public string? Address { get; set; } = string.Empty;

    [MaxLength(5)]
    public string? AddressNumber { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? City { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? State { get; set; } = string.Empty;

    [MaxLength(9)]
    public string? ZipCode { get; set; } = string.Empty;

    [MaxLength(56)]
    public string? Country { get; set; } = string.Empty;
    #endregion

    public DateTime? DateOfBirth { get; set; }

    [MaxLength(12)]
    public string? Phone { get; set; } = string.Empty;

    [MaxLength(512)]
    public string? Notes { get; set; } = string.Empty;

    public Guid CompanyId { get; set; }
    [ForeignKey(nameof(CompanyId))]
    public Company? Company { get; set; }

    public IEnumerable<Schedule>? Schedules { get; init; }
}