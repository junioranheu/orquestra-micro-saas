using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Memory;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Auth.CreateTokenJWT;
using Orquestra.Application.UseCases.Auth.GetMe;
using Orquestra.Application.UseCases.Auth.Logout;
using Orquestra.Application.UseCases.Auth.RecoverPassword;
using Orquestra.Application.UseCases.Auth.Shared;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.GetCurrentMain;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.Infrastructure.Services.Env.Models;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
        IMemoryCache memoryCache,
        IEnvService env,
        ICreateToken createToken,
        IGetCurrentMainCompanyUser getCurrentMainCompanyUser,
        ILogoutUser logoutUser,
        IRecoverPasswordUser recoverPasswordUser,
        IGetMeOutput getMeOutput
    ) : BaseController<AuthController>
{
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly IEnvService _env = env;
    private readonly ICreateToken _createToken = createToken;
    private readonly IGetCurrentMainCompanyUser _getCurrentMainCompanyUser = getCurrentMainCompanyUser;
    private readonly ILogoutUser _logoutUser = logoutUser;
    private readonly IRecoverPasswordUser _recoverPasswordUser = recoverPasswordUser;
    private readonly IGetMeOutput _getMeOutput = getMeOutput;

    [AllowAnonymous]
    [EnableRateLimiting(SystemConsts.Policies.RateLimiting)]
    [HttpPost]
    public async Task<ActionResult> Auth(AuthInput input)
    {
        if (IsUserAuth())
        {
            await _logoutUser.Execute(userIdAuth: GetUserIdAuth());
        }

        UserOutput output = await _createToken.Execute(input);

        return Ok(output);
    }

    [AllowAnonymous]
    [HttpGet("Me/Simple")]
    public ActionResult MeSimple()
    {
        bool isAuth = IsUserAuth();
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: false);
        string nameAuth = GetUserNameAuth();
        string emailAuth = GetUserEmailAuth();
        (UserRoleEnum[] userRoles, string[] userRolesStr) = GetUserRolesAuth();

        MeSimpleOutput output = new()
        {
            IsAuth = isAuth,
            UserId = userIdAuth,
            UserName = nameAuth,
            Email = emailAuth,
            Roles = userRoles,
            RolesStr = userRolesStr
        };

        return Ok(output);
    }

    [AuthorizeFilter]
    [HttpGet("Me")]
    public async Task<ActionResult> Me(bool checkExpirationDate = false)
    {
        // Misc;
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        string cacheKey = $"key_me_{userIdAuth}";

        if (_memoryCache.TryGetValue(cacheKey, out MeOutput? cachedOutput))
        {
            return Ok(cachedOutput);
        }

        const bool isAuth = true;
        string nameAuth = GetUserNameAuth();
        string emailAuth = GetUserEmailAuth();
        (UserRoleEnum[] userRoles, string[] userRolesStr) = GetUserRolesAuth();
        string? token = Request.Cookies[SystemConsts.Cookies.Auth];

        MeOutput output = await _getMeOutput.Execute(checkExpirationDate, token, userIdAuth, isAuth, nameAuth, emailAuth, userRoles, userRolesStr);

        // Cache de apenas x segundos para ajudar nas requisições repetidas;
        _memoryCache.Set(cacheKey, output, TimeSpan.FromSeconds(1.5));

        return Ok(output);
    }

    [AuthorizeFilter]
    [HttpGet("Me/CurrentMainCompany")]
    public async Task<ActionResult> MeCurrentMainCompany()
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);

        (CompanyOutput? currentMainCompany, bool _) = await _getCurrentMainCompanyUser.Execute(userIdAuth);
        CompanyOutput currentMainCompanySimple = currentMainCompany.Adapt<CompanyOutput>();

        return Ok(currentMainCompanySimple);
    }

    [AllowAnonymous]
    [HttpGet("Me/Modules")]
    public async Task<ActionResult> MeModules(Guid? userId)
    {
        if (userId is null || userId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("O usuário da requisição é inválido.");
        }

        // Current main company;
        (CompanyOutput? currentMainCompany, bool isUserAdm) = await _getCurrentMainCompanyUser.Execute(userId.GetValueOrDefault());

        if (isUserAdm)
        {
            var moduleEnum = GetEnumListWithDescriptions<ModuleEnum>();
            ModuleEnum[] outputIsAdmin = [.. moduleEnum.Select(x => x.Value)];

            return Ok(outputIsAdmin);
        }

        ModuleEnum[]? output = currentMainCompany?.UserModules ?? [];

        return Ok(output);
    }

    [AllowAnonymous]
    [HttpDelete]
    public async Task<ActionResult> Logout()
    {
        if (!IsUserAuth())
        {
            return NoContent();
        }

        await _logoutUser.Execute(userIdAuth: GetUserIdAuth());

        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("Send/RecoverPassword/{email}")]
    public async Task<IActionResult> SendRecoverPassword(string email)
    {
        if (IsUserAuth())
        {
            await _logoutUser.Execute(userIdAuth: GetUserIdAuth());
        }

        await _recoverPasswordUser.SendEmail(email);

        return Ok(true);
    }

    [AllowAnonymous]
    [HttpGet("Verify/RecoverPassword/{token}")]
    public async Task<IActionResult> VerifyRecoverPassword(string token)
    {
        if (IsUserAuth())
        {
            await _logoutUser.Execute(userIdAuth: GetUserIdAuth());
        }

        await _recoverPasswordUser.Verify(token);

        EnvOutput env = _env.GetUrls();
        string url = $"{env.UrlFrontend}/{SystemConsts.Screens.UserPasswordReset}";

        return Redirect(url);
    }
}