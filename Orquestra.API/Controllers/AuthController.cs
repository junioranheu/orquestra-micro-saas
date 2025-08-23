using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orquestra.Application.UseCases.Auth.CreateTokenJWT;
using Orquestra.Application.UseCases.Auth.Shared;
using Orquestra.Application.UseCases.Users.Shared;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ICreateToken createToken) : BaseController<AuthController>
{
    private readonly ICreateToken _createToken = createToken;

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult> Auth(AuthInput input)
    {
        if (IsUserAuth())
        {
            throw new Exception($"Usuário já está autenticado ({GetUserEmailAuth()}). Realize o logoff no sistema e tente novamente mais tarde.");
        }

        UserOutput output = await _createToken.Execute(input);

        return Ok(output);
    }

    [AllowAnonymous]
    [HttpGet("/Me")]
    public ActionResult IsAuth()
    {
        bool isAuth = IsUserAuth();

        return Ok(isAuth);
    }
}