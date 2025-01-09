using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orquestra.Application.UseCases.Users.Create;
using Orquestra.Application.UseCases.Users.Shared;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(ICreateUser create) : BaseController<UserController>
{
    private readonly ICreateUser _create = create;

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<UserOutput>> Create([FromForm] UserInput input)
    {
        UserOutput output = await _create.Execute(input);
        return output;
    }
}