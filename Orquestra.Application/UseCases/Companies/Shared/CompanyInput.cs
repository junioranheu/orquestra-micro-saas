using Microsoft.AspNetCore.Http;
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Companies.Shared;

public sealed class CompanyInput
{
    #region basic
    public Guid? CompanyId { get; set; } = Guid.Empty;

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;
     
    public CompanyTypeEnum CompanyType { get; set; }
    #endregion

    #region location
    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string? ZipCode { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;
    #endregion

    #region customization
    public IFormFile? LogoFormFile { get; set; }

    public string? LogoContentType { get; set; }

    public string? Color { get; set; } = string.Empty;

    public ModuleEnum[]? Modules { get; set; } = [];
    #endregion

    public bool Status { get; set; }
}