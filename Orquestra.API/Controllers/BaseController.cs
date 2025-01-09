using Microsoft.AspNetCore.Mvc;
using Orquestra.Domain.Enums;
using System.Security.Claims;
using static junioranheu_utils_package.Fixtures.Get;

namespace Orquestra.API.Controllers;

public abstract class BaseController<T> : Controller
{
    protected bool IsAuth()
    {
        if (User.Identity is null)
        {
            return false;
        }

        return User.Identity.IsAuthenticated;
    }

    protected Guid? GetUserId()
    {
        if (!IsAuth())
        {
            return Guid.Empty;
        }

        string id = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        if (string.IsNullOrEmpty(id))
        {
            return null;
        }

        Guid guid = Guid.Parse(id);

        return guid;
    }

    protected string GetUserName()
    {
        if (!IsAuth())
        {
            return string.Empty;
        }

        string name = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        return name;
    }

    protected string GetUserEmail()
    {
        if (!IsAuth())
        {
            return string.Empty;
        }

        string email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        return email;
    }

    protected (UserRoleEnum[] userRolesEnum, string[] userRolesStr) GetUserRoles()
    {
        if (!IsAuth())
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
                roleEnumsStr.Add(ObterDescricaoEnum(userRole));
            }
        }

        return ([.. roleEnums], [.. roleEnumsStr]);
    }
}