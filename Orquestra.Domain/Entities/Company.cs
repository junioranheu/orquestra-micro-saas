using Microsoft.EntityFrameworkCore;
using Orquestra.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Orquestra.Domain.Entities;

[Index(nameof(Email))]
[Index(nameof(Name))]
public sealed class Company: Audit
{
    [Key]
    public Guid CompanyId { get; set; }

    #region basic
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(12)]
    public string Phone { get; set; } = string.Empty;

    public CompanyTypeEnum CompanyType { get; set; }
    #endregion

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

    #region customization
    public byte[]? Logo { get; set; }

    public string? LogoContentType { get; set; }

    [MaxLength(20)]
    public string? Color { get; set; } = string.Empty;
    #endregion

    #region subscription
    public CompanySituationEnum CompanySituation { get; set; } = CompanySituationEnum.RegisteredButWithoutAnyModules;

    public DateTime? PlanStartDate { get; set; } = null;

    public DateTime? PlanEndDate { get; set; } = null;

    public ModuleEnum[]? Modules { get; set; } = [];
    #endregion

    public IEnumerable<CompanyUser>? CompanyUsers { get; init; }

    public IEnumerable<Schedule>? Schedules { get; init; }

    public IEnumerable<Client>? Clients { get; init; }
}