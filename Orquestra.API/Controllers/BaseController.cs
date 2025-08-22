using Microsoft.AspNetCore.Mvc;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Enums;
using System.Security.Claims;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.API.Controllers;

public abstract class BaseController<T> : Controller
{
    protected bool IsUserAuth()
    {
        if (User is null || User.Identity is null)
        {
            return false;
        }

        return User.Identity.IsAuthenticated;
    }

    protected Guid GetUserIdAuth(bool throwExceptionIfNotAuth = false)
    {
        if (!IsUserAuth())
        {
            ThrowEx(throwExceptionIfNotAuth);
            return Guid.Empty;
        }

        string id = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        if (string.IsNullOrEmpty(id))
        {
            ThrowEx(throwExceptionIfNotAuth);
            return Guid.Empty;
        }

        Guid guid = Guid.Parse(id);

        return guid;

        static void ThrowEx(bool throwExceptionIfNotAuth)
        {
            if (!throwExceptionIfNotAuth)
            {
                return;
            }

            throw new Exception(SystemConsts.Warn_Simple_UserNotAuth);
        }
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
            return ([], []);
        }

        string[]? roleClaims = User.FindAll(ClaimTypes.Role)?.Select(c => c.Value).ToArray();

        if (roleClaims?.Length < 1 || roleClaims is null)
        {
            return ([], []);
        }

        List<UserRoleEnum> roleEnums = [];
        List<string> roleEnumsStr = [];

        foreach (var role in roleClaims)
        {
            if (Enum.TryParse<UserRoleEnum>(role, true, out var userRole))
            {
                roleEnums.Add(userRole);
                roleEnumsStr.Add(GetEnumDesc(userRole));
            }
        }

        return ([.. roleEnums], [.. roleEnumsStr]);
    }
}