using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Auth.Shared;
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
        IVerifyUser verify
    ) : BaseController<UserController>
{
    private readonly IEnvService _env = env;
    private readonly IGetUser _get = get;
    private readonly ICreateUser _create = create;
    private readonly IUpdateUser _update = update;
    private readonly IVerifyUser _verify = verify;

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
            throw new UnauthorizedAccessException("Não é póssível criar uma nova conta porque você já está autenticado no sistema.");
        }

        await _create.Execute(input);

        return NoContent();
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
        string url = $"{env.UrlFrontend}/{SystemConsts.ScreenUserHasBeenVerified}";

        return Redirect(url);
    }
}