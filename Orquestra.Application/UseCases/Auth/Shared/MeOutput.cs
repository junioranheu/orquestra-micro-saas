using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.Users.Shared;

namespace Orquestra.Application.UseCases.Auth.Shared;

public sealed class MeOutput
{
    public bool IsAuth { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string[] Roles { get; set; } = [];
    public UserOutput? User { get; set; }
    public List<CompanyOutput>? Companies { get; set; }
    public CompanyOutput? CurrentMainCompany { get; set; }
}