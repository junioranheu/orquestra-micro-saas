using Microsoft.AspNetCore.Mvc;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Enums;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.API.Controllers;

public abstract class BaseController<T> : Controller
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected bool IsUserAuth()
    {
        if (User is null || User.Identity is null)
        {
            return false;
        }

        return User.Identity.IsAuthenticated;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected Guid GetUserIdAuth(bool throwExceptionIfNotAuth = false)
    {
        string? id = User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id.AsSpan(), out Guid guid))
        {
            if (throwExceptionIfNotAuth)
            {
                throw new UnauthorizedAccessException(SystemConsts.Warnings.NotAuthSimpleUser);
            }

            return Guid.Empty;
        }

        return guid;
    }

    protected string GetUserNameAuth()
    {
        if (!IsUserAuth())
        {
            return string.Empty;
        }

        string name = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

        return name;
    }

    protected string GetUserEmailAuth()
    {
        if (!IsUserAuth())
        {
            return string.Empty;
        }

        string email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        return email;
    }

    protected (UserRoleEnum[] userRolesEnum, string[] userRolesStr) GetUserRolesAuth()
    {
        if (!IsUserAuth())
        {
            return (Array.Empty<UserRoleEnum>(), Array.Empty<string>());
        }

        List<UserRoleEnum> enums = [];
        List<string> names = [];

        foreach (var claim in User.FindAll(ClaimTypes.Role))
        {
            if (Enum.TryParse(claim.Value, true, out UserRoleEnum userRole))
            {
                enums.Add(userRole);
                names.Add(GetEnumDesc(userRole));
            }
        }

        return (enums.ToArray(), names.ToArray());
    }
}