using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Orquestra.API.Filters.Base;
using Orquestra.Application.UseCases.CompanyUsers.GetCurrentMain;
using Orquestra.Domain.Enums;

namespace Orquestra.API.Filters;

#region attribute
public sealed class AuthorizeFilterAttribute : TypeFilterAttribute
{
    public AuthorizeFilterAttribute() : base(typeof(AuthorizeFilter))
    {
        Arguments = [Array.Empty<UserRoleEnum>(), Array.Empty<ModuleEnum>()];
    }

    public AuthorizeFilterAttribute(UserRoleEnum[] roles) : base(typeof(AuthorizeFilter))
    {
        Arguments = [roles ?? [], Array.Empty<ModuleEnum>()];
    }

    public AuthorizeFilterAttribute(ModuleEnum[] modules) : base(typeof(AuthorizeFilter))
    {
        Arguments = [Array.Empty<UserRoleEnum>(), modules ?? []];
    }

    public AuthorizeFilterAttribute(UserRoleEnum[] roles, ModuleEnum[] modules) : base(typeof(AuthorizeFilter))
    {
        Arguments = [roles ?? [], modules ?? []];
    }
}
#endregion

public sealed class AuthorizeFilter(UserRoleEnum[] rolesRequired, ModuleEnum[] modulesRequired, IGetCurrentMainCompanyUser getCurrentMainCompanyUser) : IAsyncAuthorizationFilter
{
    private readonly UserRoleEnum[] _rolesRequired = rolesRequired ?? [];
    private readonly ModuleEnum[] _modulesRequired = modulesRequired ?? [];
    private readonly IGetCurrentMainCompanyUser _getCurrentMainCompanyUser = getCurrentMainCompanyUser;

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (!IsAuthenticated(context))
        {
            return;
        }

        (Guid? userId, string _, UserRoleEnum[] rolesFromToken) = new BaseFilter().GetUserInfo(context);

        if (userId is null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        bool isAdmin = rolesFromToken.Any(x => x == UserRoleEnum.Administrator);

        if (isAdmin)
        {
            return;
        }

        bool hasRoles = CheckRoles(context, rolesFromToken ?? [], _rolesRequired);

        if (!hasRoles)
        {
            return;
        }

        bool hasModules = await CheckModules(context, userId.GetValueOrDefault(), _modulesRequired);

        if (!hasModules)
        {
            return;
        }
    }

    #region extras
    private static bool IsAuthenticated(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return false;
        }

        return true;
    }

    private static bool CheckRoles(AuthorizationFilterContext context, UserRoleEnum[] userRoles, UserRoleEnum[] requiredRoles)
    {
        if (requiredRoles is null || requiredRoles.Length == 0)
        {
            return true;
        }

        bool ok = userRoles.Any(x => requiredRoles.Contains(x));

        if (!ok)
        {
            context.Result = new ObjectResult("Você não tem permissão de acesso.")
            {
                StatusCode = StatusCodes.Status403Forbidden
            };

            return false;
        }

        return true;
    }

    private async Task<bool> CheckModules(AuthorizationFilterContext context, Guid userId, ModuleEnum[] requiredModules)
    {
        if (requiredModules is null || requiredModules.Length == 0)
        {
            return true;
        }

        (ModuleEnum[] modules, List<string> _) = await _getCurrentMainCompanyUser.GetCurrentModules(userId);

        bool ok = modules.Any(x => requiredModules.Contains(x));

        if (!ok)
        {
            context.Result = new ObjectResult("Você não possui acesso a este módulo.")
            {
                StatusCode = StatusCodes.Status403Forbidden
            };

            return false;
        }

        return true;
    }
    #endregion
}