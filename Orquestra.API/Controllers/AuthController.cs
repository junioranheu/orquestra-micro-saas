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
            throw new Exception("Este usuário já está autenticado");
        }

        UserOutput output = await _createToken.Execute(input);
        return output;
    }
}
