using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Auth.Logout;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Application.UseCases.Users.Create;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Application.UseCases.Users.Update;
using Orquestra.Application.UseCases.Users.Verify;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.Infrastructure.Services.Env.Models;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(
        IEnvService env,
        IGetUser get,
        ICreateUser create,
        IUpdateUser update,
        IVerifyUser verify,
        ILogoutUser logout
    ) : BaseController<UserController>
{
    private readonly IEnvService _env = env;
    private readonly IGetUser _get = get;
    private readonly ICreateUser _create = create;
    private readonly IUpdateUser _update = update;
    private readonly IVerifyUser _verify = verify;
    private readonly ILogoutUser _logout = logout;

    [AuthorizeFilter(UserRoleEnum.Administrator, UserRoleEnum.Maintainer)]
    [HttpGet]
    public async Task<ActionResult> Get([FromQuery] PaginationInput paginationInput)
    {
        (IEnumerable<UserOutput> output, int count) = await _get.Execute(paginationInput);

        return Ok(new { output, count });
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult> Create(UserInput input)
    {
        if (IsUserAuth())
        {
            await _logout.Execute(userIdAuth: GetUserIdAuth());
        }

        await _create.Execute(input);

        return Ok(true);
    }

    [AuthorizeFilter]
    [HttpPut]
    public async Task<ActionResult> Update([FromForm] UserInput input)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        UserOutput output = await _update.Execute(userIdAuth, input);

        return Ok(output);
    }

    [AllowAnonymous]
    [HttpGet("Verify/{token}")]
    public async Task<IActionResult> Verify(string token)
    {
        await _verify.Execute(token);

        EnvOutput env = _env.GetUrls();
        string url = $"{env.UrlFrontend}/{SystemConsts.Screens.UserVerified}";

        return Redirect(url);
    }
}