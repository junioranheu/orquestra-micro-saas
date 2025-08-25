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
    public List<CompanySimpleOutput>? Companies { get; set; }
}