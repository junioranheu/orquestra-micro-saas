using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Application.UseCases.Users.Create;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Application.UseCases.Users.Update;
using Orquestra.Domain.Enums;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IGetUser get, ICreateUser create, IUpdateUser update) : BaseController<UserController>
{
    private readonly IGetUser _get = get;
    private readonly ICreateUser _create = create;
    private readonly IUpdateUser _update = update;

    [AuthorizeFilter(UserRoleEnum.Admin, UserRoleEnum.Maintainer)]
    [HttpGet]
    public async Task<ActionResult> Create([FromQuery] PaginationInput paginationInput)
    {
        (IEnumerable<UserOutput> linq, int count) = await _get.Execute(paginationInput);
        return Ok(new { linq, count });
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult> Create([FromForm] UserInput input)
    {
        UserOutput output = await _create.Execute(input);
        return Ok(output);
    }

    [AuthorizeFilter]
    [HttpPut]
    public async Task<ActionResult> Update([FromForm] UserInput input)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        UserOutput output = await _update.Execute(userIdAuth, input);

        return Ok(output);
    }
}