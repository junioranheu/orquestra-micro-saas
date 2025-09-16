using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Auth.CreateRefreshTokenJWT;
using Orquestra.Application.UseCases.Auth.CreateTokenJWT;
using Orquestra.Application.UseCases.Auth.GetRefreshTokenJWT;
using Orquestra.Application.UseCases.Auth.Shared;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.GetCurrentMain;
using Orquestra.Application.UseCases.CompanyUsers.GetModule;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Auth.Token;
using System.IdentityModel.Tokens.Jwt;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
        ICreateToken createToken,
        ICreateRefreshToken createRefreshToken,
        IJwtTokenGenerator jwtTokenGenerator,
        IGetRefreshToken getRefreshToken,
        IGetCurrentMainCompanyUser getCurrentMainCompanyUser,
        IGetModuleCompanyUser getModuleCompanyUser
    ) : BaseController<AuthController>
{
    private readonly ICreateToken _createToken = createToken;
    private readonly ICreateRefreshToken _createRefreshToken = createRefreshToken;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IGetRefreshToken _getRefreshToken = getRefreshToken;
    private readonly IGetCurrentMainCompanyUser _getCurrentMainCompanyUser = getCurrentMainCompanyUser;
    private readonly IGetModuleCompanyUser _getModuleCompanyUser = getModuleCompanyUser;

    [AllowAnonymous]
    [EnableRateLimiting(SystemConsts.PolicyRateLimiting)]
    [HttpPost]
    public async Task<ActionResult> Auth(AuthInput input)
    {
        if (IsUserAuth())
        {
            await LogoutAsync();
        }

        UserOutput output = await _createToken.Execute(input);

        return Ok(output);
    }

    [AllowAnonymous]
    [EnableRateLimiting(SystemConsts.PolicyRateLimiting)]
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
    [EnableRateLimiting(SystemConsts.PolicyRateLimiting)]
    [HttpGet("Me")]
    public async Task<ActionResult> Me()
    {
        // Misc;
        const bool isAuth = true;
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        string nameAuth = GetUserNameAuth();
        string emailAuth = GetUserEmailAuth();
        (UserRoleEnum[] userRoles, string[] userRolesStr) = GetUserRolesAuth();

        // Current main company;
        (CompanyOutput? currentMainCompany, bool isUserAdm) = await _getCurrentMainCompanyUser.Execute(userIdAuth);
        CompanySimpleOutput currentMainCompanySimple = currentMainCompany.Adapt<CompanySimpleOutput>();

        // Token;
        string? token = Request.Cookies[SystemConsts.CookieName];
        JwtSecurityToken jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        (_, _, DateTime validTo) = _jwtTokenGenerator.IsTokenExpiringSoonOrHasAlreadyExpired(jwtToken);

        // Refresh token;
        RefreshToken? refreshToken = await _getRefreshToken.GetLatestNotRevokedToken(userIdAuth);

        // Output;
        MeOutput output = new()
        {
            IsAuth = isAuth,
            UserId = userIdAuth,
            UserName = nameAuth,
            Email = emailAuth,
            Roles = userRoles,
            RolesStr = userRolesStr,
            CurrentMainCompany = currentMainCompanySimple,
            TokenExpirationDate = validTo,
            RefreshTokenExpirationDate = refreshToken?.ExpiredDate.GetValueOrDefault() ?? DateTime.MinValue,
            IsUserAdmOfCurrentMainCompany = isUserAdm
        };

        // Módulos;
        if (currentMainCompany is not null)
        {
            (ModuleEnum[] modules, List<string> modulesStr) = await _getModuleCompanyUser.Execute(userIdAuth, companyId: currentMainCompany.CompanyId);

            output.CurrentMainCompany.UserModules = modules;
            output.CurrentMainCompany.UserModulesStr = modulesStr;
        }

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

        await LogoutAsync();

        return NoContent();
    }

    private async Task LogoutAsync()
    {
        CookieOptions cookieOptions = _jwtTokenGenerator.GetCookieOptions();
        HttpContext.Response.Cookies.Delete(SystemConsts.CookieName, cookieOptions);

        await _createRefreshToken.Update(userIdAuth: GetUserIdAuth(), mustCheckForValidRefreshTokens: true);
    }
}