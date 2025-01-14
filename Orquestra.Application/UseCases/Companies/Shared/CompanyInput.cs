using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Companies.Shared;

public sealed class CompanyInput
{
    #region basic
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public CompanyTypeEnum Type { get; set; }
    #endregion

    #region location
    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string ZipCode { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;
    #endregion

    #region customization
    public string LogoUrl { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;
    #endregion

    #region subscription
    public PlanTypeEnum PlanType { get; set; }
    #endregion
}