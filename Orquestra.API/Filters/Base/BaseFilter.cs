using Microsoft.AspNetCore.Mvc.Filters;
using Orquestra.Domain.Enums;
using System.Security.Claims;

namespace Orquestra.API.Filters.Base;

public sealed class BaseFilter
{
#pragma warning disable CA1822
    internal (Guid? userId, string email, UserRoleEnum[] roles) GetUserInfo(dynamic context)
#pragma warning restore CA1822 
    {
        if (context is ActionExecutedContext actionExecutedContext)
        {
            return BaseGetUserInfo(actionExecutedContext);
        }
        else if (context is AuthorizationFilterContext authorizationFilterContext)
        {
            return BaseGetUserInfo(authorizationFilterContext);
        }
        else if (context is ExceptionContext exceptionContext)
        {
            return BaseGetUserInfo(exceptionContext);
        }

        return (null, string.Empty, []);

        static (Guid? userId, string email, UserRoleEnum[] roles) BaseGetUserInfo(dynamic context)
        {
            if (context.HttpContext.User.Identity!.IsAuthenticated)
            {
                ClaimsPrincipal? user = (ClaimsPrincipal)context.HttpContext.User;

                if (user is null)
                {
                    return (null, string.Empty, []);
                }

                string userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                string email = user?.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
                string[] rolesStr = user?.FindAll(ClaimTypes.Role)?.Select(claim => claim.Value).ToArray() ?? [];

                List<UserRoleEnum> rolesList = [];

                foreach (var item in rolesStr)
                {
                    UserRoleEnum role = Enum.Parse<UserRoleEnum>(item);
                    rolesList.Add(role);
                }

                UserRoleEnum[] roles = [.. rolesList];

                return (Guid.Parse(userId), email, roles);
            }

            return (null, string.Empty, []);
        }
    }
}