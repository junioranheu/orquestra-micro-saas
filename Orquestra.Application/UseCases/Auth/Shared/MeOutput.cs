using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Auth.Shared;

public class MeSimpleOutput
{
    public bool IsAuth { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRoleEnum[] Roles { get; set; } = [];
    public string[] RolesStr { get; set; } = [];
}

public sealed class MeOutput : MeSimpleOutput
{
    public CompanyOutput? CurrentMainCompany { get; set; }
    public DateTime TokenExpirationDate { get; set; }
    public DateTime RefreshTokenExpirationDate { get; set; }
    public bool IsUserAdmOfCurrentMainCompany { get; set; }
}