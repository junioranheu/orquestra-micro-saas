using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Auth.Shared;

public class MeSimpleOutput
{
    public bool IsAuth { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public UserRoleEnum[] Roles { get; set; } = [];
    public string[] RolesStr { get; set; } = [];
}

public sealed class MeOutput : MeSimpleOutput
{
    public CompanySimpleOutput? CurrentMainCompany { get; set; }
    public ModuleEnum[]? Modules { get; set; } = [];
    public List<string> ModulesStr { get; set; } = [];
    public DateTime TokenExpirationDate { get; set; }
    public DateTime RefreshTokenExpirationDate { get; set; }
}