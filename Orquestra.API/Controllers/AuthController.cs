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
    public async Task<ActionResult<UserOutput>> Auth(AuthInput input)
    {
        if (IsAuth())
        {
            throw new Exception("Usuário já está autenticado. Realize o logoff no sistema e tente novamente mais tarde.");
        }

        UserOutput output = await _createToken.Execute(input);
        return Ok(output);
    }
}
