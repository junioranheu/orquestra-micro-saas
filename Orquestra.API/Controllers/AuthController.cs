using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Auth.CreateRefreshTokenJWT;
using Orquestra.Application.UseCases.Auth.CreateTokenJWT;
using Orquestra.Application.UseCases.Auth.Shared;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Enums;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
        ICreateToken createToken,
        ICreateRefreshToken createRefreshToken,
        IGetUser getUser,
        IGetCompany getCompany
    ) : BaseController<AuthController>
{
    private readonly ICreateToken _createToken = createToken;
    private readonly ICreateRefreshToken _createRefreshToken = createRefreshToken;
    private readonly IGetUser _getUser = getUser;
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
    [HttpGet]
    public ActionResult IsAuth()
    {
        bool isAuth = IsUserAuth();

        return Ok(isAuth);
    }

    [AuthorizeFilter]
    [HttpGet("Me")]
    public async Task<ActionResult> Me()
    {
        bool isAuth = IsUserAuth();
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        string nameAuth = GetUserNameAuth();
        (UserRoleEnum[] _, string[] userRolesStr) = GetUserRolesAuth();
        UserOutput userOutput = await _getUser.Execute(userId: userIdAuth);
        List<CompanyOutput>? companyOutput = await _getCompany.Execute(userId: userIdAuth);
        CompanyOutput? currentMainCompany = companyOutput?.FirstOrDefault(x => x.CompanyUsers!.Any(y => y.IsCurrentMainCompanyUser));

        MeOutput output = new()
        {
            IsAuth = isAuth,
            UserId = userIdAuth,
            UserName = nameAuth,
            Roles = userRolesStr,
            User = userOutput,
            Companies = companyOutput,
            CurrentMainCompany = currentMainCompany
        };

        return Ok(output);
    }

    [AllowAnonymous]
    [HttpDelete]
    public async Task<ActionResult> Logout()
    {
        if (!IsUserAuth())
        {
            return BadRequest("Você não está autenticado.");
        }

        HttpContext.Response.Cookies.Delete(SystemConsts.CookieName);

        await _createRefreshToken.Update(userIdAuth: GetUserIdAuth(), mustCheckForValidRefreshTokens: true);

        return Ok();
    }
}