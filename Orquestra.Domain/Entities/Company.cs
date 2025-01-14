using Microsoft.EntityFrameworkCore;
using Orquestra.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Orquestra.Domain.Entities;

[Index(nameof(Email))]
public sealed class Company: Audit
{
    [Key]
    public Guid CompanyId { get; set; }

    #region basic
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(12)]
    public string Phone { get; set; } = string.Empty;

    public CompanyTypeEnum Type { get; set; }
    #endregion

    #region location
    [MaxLength(255)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [MaxLength(100)]
    public string State { get; set; } = string.Empty;

    [MaxLength(9)]
    public string ZipCode { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Country { get; set; } = string.Empty;
    #endregion

    #region customization
    public string LogoUrl { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Color { get; set; } = string.Empty;
    #endregion

    #region subscription
    public CompanySituationEnum CompanySituation { get; set; }

    public PlanTypeEnum PlanType { get; set; }

    public DateTime PlanStartDate { get; set; }

    public DateTime PlanEndDate { get; set; }
    #endregion

    public IEnumerable<CompanyUser>? CompanyUsers { get; init; }
}