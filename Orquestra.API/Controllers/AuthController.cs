using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Auth.CreateTokenJWT;
using Orquestra.Application.UseCases.Auth.GetRefreshTokenJWT;
using Orquestra.Application.UseCases.Auth.Logout;
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
        IJwtTokenGenerator jwtTokenGenerator,
        IGetRefreshToken getRefreshToken,
        IGetCurrentMainCompanyUser getCurrentMainCompanyUser,
        IGetModuleCompanyUser getModuleCompanyUser,
        ILogoutUser logoutUser
    ) : BaseController<AuthController>
{
    private readonly ICreateToken _createToken = createToken;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IGetRefreshToken _getRefreshToken = getRefreshToken;
    private readonly IGetCurrentMainCompanyUser _getCurrentMainCompanyUser = getCurrentMainCompanyUser;
    private readonly IGetModuleCompanyUser _getModuleCompanyUser = getModuleCompanyUser;
    private readonly ILogoutUser _logoutUser = logoutUser;

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
    public async Task<ActionResult> Me()
    {
        // Misc;
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);

        const bool isAuth = true;
        string nameAuth = GetUserNameAuth();
        string emailAuth = GetUserEmailAuth();
        (UserRoleEnum[] userRoles, string[] userRolesStr) = GetUserRolesAuth();

        // Current main company;
        (CompanyOutput? currentMainCompany, bool isUserAdm) = await _getCurrentMainCompanyUser.Execute(userIdAuth);
        CompanyOutput currentMainCompanySimple = currentMainCompany.Adapt<CompanyOutput>();

        // Token;
        string? token = Request.Cookies[SystemConsts.Cookies.Auth];
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

    [AuthorizeFilter]
    [HttpGet("Me/CurrentMainCompany")]
    public async Task<ActionResult> MeCurrentMainCompany()
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);

        (CompanyOutput? currentMainCompany, bool isUserAdm) = await _getCurrentMainCompanyUser.Execute(userIdAuth);
        CompanyOutput currentMainCompanySimple = currentMainCompany.Adapt<CompanyOutput>();

        if (currentMainCompanySimple is not null)
        {
            currentMainCompanySimple.IsAdm = isUserAdm;
        }

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
        (CompanyOutput? currentMainCompany, bool _) = await _getCurrentMainCompanyUser.Execute(userId.GetValueOrDefault());
        ModuleEnum[]? output = currentMainCompany?.Modules ?? [];

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
}