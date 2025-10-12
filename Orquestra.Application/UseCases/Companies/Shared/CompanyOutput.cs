using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Companies.Shared;

public sealed class CompanyOutput
{
    public Guid CompanyId { get; set; }

    #region basic
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public CompanyTypeEnum CompanyType { get; set; }
    #endregion

    #region location
    public string? Address { get; set; } = string.Empty;

    public string? AddressNumber { get; set; } = string.Empty;

    public string? City { get; set; } = string.Empty;

    public string? State { get; set; } = string.Empty;

    public string? ZipCode { get; set; } = string.Empty;

    public string? Country { get; set; } = string.Empty;
    #endregion

    #region customization
    public string? LogoBase64 { get; set; } // Base64;

    public string? LogoContentType { get; set; }

    public string? Color { get; set; } = string.Empty;
    #endregion

    #region subscription
    public CompanySituationEnum CompanySituation { get; set; }

    public DateTime? PlanStartDate { get; set; }

    public DateTime? PlanEndDate { get; set; }

    public ModuleEnum[]? Modules { get; set; } = [];
    #endregion

    #region extras
    public List<string> ModulesStr { get; set; } = [];

    public bool Status { get; set; }

    public DateTime CreatedDate { get; set; }

    public IEnumerable<CompanyUserOutput>? CompanyUsers { get; init; }

    public int AmountOfClients { get; set; } = 0;

    public string? CompanyTypeStr { get; set; }

    public string? CompanySituationStr { get; set; }

    public ModuleEnum[]? UserModules { get; set; } = [];

    public List<string> UserModulesStr { get; set; } = [];

    public bool IsAdm { get; set; } = false;
    #endregion
}

public sealed class CompanyModulesOutput
{
    public required CompanyOutput Company { get; set; }
    public ModuleEnum[]? Modules { get; set; } = [];
    public List<string> ModulesStr { get; set; } = [];
}