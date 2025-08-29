using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Auth.CreateRefreshTokenJWT;
using Orquestra.Application.UseCases.Auth.CreateTokenJWT;
using Orquestra.Application.UseCases.Auth.GetRefreshTokenJWT;
using Orquestra.Application.UseCases.Auth.Shared;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.Companies.Shared;
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
        IGetCompany getCompany
    ) : BaseController<AuthController>
{
    private readonly ICreateToken _createToken = createToken;
    private readonly ICreateRefreshToken _createRefreshToken = createRefreshToken;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IGetRefreshToken _getRefreshToken = getRefreshToken;
    private readonly IGetCompany _getCompany = getCompany;

#if DEBUG
    [AllowAnonymous]
    [HttpPost("AuthTeste")]
    public async Task<ActionResult> AuthTeste()
    {
        if (IsUserAuth())
        {
            throw new Exception($"Você já está autenticado.");
        }

        AuthInput input = new()
        {
            Email = "junioranheu@gmail.com",
            Password = "Junior30@@"
        };

        UserOutput output = await _createToken.Execute(input);

        return Ok(output);
    }
#endif

    [AllowAnonymous]
    [EnableRateLimiting(SystemConsts.PolicyRateLimiting)]
    [HttpPost]
    public async Task<ActionResult> Auth(AuthInput input)
    {
        if (IsUserAuth())
        {
            throw new Exception($"Você já está autenticado.");
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
        (UserRoleEnum[] userRoles, string[] userRolesStr) = GetUserRolesAuth();

        MeSimpleOutput output = new()
        {
            IsAuth = isAuth,
            UserId = userIdAuth,
            UserName = nameAuth,
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
        (UserRoleEnum[] userRoles, string[] userRolesStr) = GetUserRolesAuth();

        // Companies;
        List<CompanyOutput>? companyOutput = await _getCompany.Execute(userId: userIdAuth);
        List<CompanySimpleOutput> companySimpleOutput = companyOutput.Adapt<List<CompanySimpleOutput>>();

        // Current main company;
        CompanyOutput? currentMainCompany = companyOutput?.
                                            Where(x => x.CompanyUsers!.Any(
                                                y => y.UserId == userIdAuth && y.IsCurrentMainCompanyUser == true && x.Status == true
                                            )).FirstOrDefault();

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
            Roles = userRoles,
            RolesStr = userRolesStr,
            CurrentMainCompany = currentMainCompanySimple,
            Companies = companySimpleOutput,
            TokenExpirationDate = validTo,
            RefreshTokenExpirationDate = refreshToken?.ExpiredDate.GetValueOrDefault() ?? DateTime.MinValue
        };

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

        CookieOptions cookieOptions = _jwtTokenGenerator.GetCookieOptions();
        HttpContext.Response.Cookies.Delete(SystemConsts.CookieName, cookieOptions);
        await _createRefreshToken.Update(userIdAuth: GetUserIdAuth(), mustCheckForValidRefreshTokens: true);

        return NoContent();
    }
}