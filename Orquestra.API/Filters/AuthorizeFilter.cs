using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Orquestra.API.Filters.Base;
using Orquestra.Domain.Enums;

namespace Orquestra.API.Filters;

public sealed class AuthAttribute : TypeFilterAttribute
{
    public AuthAttribute(params UserRoleEnum[] roles) : base(typeof(AuthorizeFilter))
    {
        Arguments = [roles];
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class AuthorizeFilter(params UserRoleEnum[] roles) : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly UserRoleEnum[] _rolesRequired = roles;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (IsAuth(context))
        {
            (Guid? _, string _, UserRoleEnum[] roles) = new BaseFilter().GetUserInfo(context);
            CheckHasAccess(context, roles, _rolesRequired);
        }
    }

    private static bool IsAuth(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.User.Identity!.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return false;
        }

        return true;
    }

    private static bool CheckHasAccess(AuthorizationFilterContext context, UserRoleEnum[] roles, UserRoleEnum[] _rolesRequired)
    {
        if (_rolesRequired.Length == 0)
        {
            return true;
        }

        bool hasAccess = roles!.Any(x => _rolesRequired.Any(y => x == y));

        if (!hasAccess)
        {
            context.Result = new ObjectResult("Você não tem permissão para acessar este recurso.")
            {
                StatusCode = StatusCodes.Status403Forbidden
            };

            return false;
        }

        return true;
    }
}