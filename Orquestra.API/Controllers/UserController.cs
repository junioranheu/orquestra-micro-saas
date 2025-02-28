using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Users.Create;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Application.UseCases.Users.Update;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(ICreateUser create, IUpdateUser update) : BaseController<UserController>
{
    private readonly ICreateUser _create = create;
    private readonly IUpdateUser _update = update;

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<UserOutput>> Create([FromForm] UserInput input)
    {
        UserOutput output = await _create.Execute(input);
        return output;
    }

    [AuthorizeFilter]
    [HttpPut]
    public async Task<ActionResult<UserOutput>> Update([FromForm] UserInput input)
    {
        Guid userId = GetUserId(throwExceptionIfNotAuth: true);
        UserOutput output = await _update.Execute(userId, input);

        return output;
    }
}