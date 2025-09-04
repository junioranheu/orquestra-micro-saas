using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Orquestra.Application.UseCases.Companies.Shared;

public sealed class CompanyOutput
{
    public Guid CompanyId { get; set; }

    #region basic
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(12)]
    public string Phone { get; set; } = string.Empty;

    public CompanyTypeEnum CompanyType { get; set; }
    #endregion

    #region location
    [MaxLength(255)]
    public string StreetAdress { get; set; } = string.Empty;

    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [MaxLength(100)]
    public string State { get; set; } = string.Empty;

    [MaxLength(9)]
    public string? ZipCode { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Country { get; set; } = string.Empty;
    #endregion

    #region customization
    public string? LogoUrl { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Color { get; set; } = string.Empty;
    #endregion

    #region subscription
    public CompanySituationEnum CompanySituation { get; set; }

    public PlanTypeEnum PlanType { get; set; }

    public DateTime PlanStartDate { get; set; }

    public DateTime PlanEndDate { get; set; }

    public ModuleEnum[] Modules { get; set; } = [];
    #endregion

    #region extras
    public bool Status { get; set; }

    public DateTime CreatedDate { get; set; }

    public IEnumerable<CompanyUserOutput>? CompanyUsers { get; init; }
    #endregion
}

public sealed class CompanySimpleOutput
{
    public Guid CompanyId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public CompanyTypeEnum CompanyType { get; set; }

    public CompanySituationEnum CompanySituation { get; set; }

    public PlanTypeEnum PlanType { get; set; }

    public DateTime PlanStartDate { get; set; }

    public DateTime PlanEndDate { get; set; }

    public ModuleEnum[] Modules { get; set; } = [];
}

public sealed class CompanyModulesOutput
{
    public required CompanySimpleOutput Company { get; set; }
    public ModuleEnum[] Modules { get; set; } = [];
    public List<string> ModulesStr { get; set; } = [];
}