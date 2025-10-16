using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.CompanyUsers.Delete;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.CompanyUsers.Invite;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Application.UseCases.CompanyUsers.Update;
using Orquestra.Application.UseCases.CompanyUsers.UpdateCurrentMain;
using Orquestra.Application.UseCases.CompanyUsers.Verify;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.Infrastructure.Services.Env.Models;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyUserController(
        IEnvService env,
        IInviteCompanyUser invite,
        IGetCompanyUserByCompanyId getCompanyUserByCompanyId,
        IVerifyCompanyUser verify,
        IUpdateCurrentMainCompanyUser updateCurrentMainCompanyUser,
        IUpdateCompanyUser updateModuleCompanyUser,
        IDeleteCompanyUser delete
    ) : BaseController<CompanyUserController>
{
    private readonly IEnvService _env = env;
    private readonly IInviteCompanyUser _invite = invite;
    private readonly IGetCompanyUserByCompanyId _getCompanyUserByCompanyId = getCompanyUserByCompanyId;
    private readonly IVerifyCompanyUser _verify = verify;
    private readonly IUpdateCurrentMainCompanyUser _updateCurrentMainCompanyUser = updateCurrentMainCompanyUser;
    private readonly IUpdateCompanyUser _updateModuleCompanyUser = updateModuleCompanyUser;
    private readonly IDeleteCompanyUser _delete = delete;

    [AuthorizeFilter]
    [HttpPost("InviteUser")]
    public async Task<ActionResult> InviteUser(CompanyUserInviteInput input)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _invite.Execute(userIdAuth, companyId: input.CompanyId, email: input.Email, isFirstAdministrator: false);

        return Ok(true);
    }

    [AuthorizeFilter]
    [HttpGet("GetAllByCompanyId")]
    public async Task<ActionResult> GetAllByCompanyId([FromQuery] PaginationInput paginationInput, [FromQuery] CompanyUserFilterInput input)
    {
        if (input.CompanyId == Guid.Empty)
        {
            return NoContent();
        }

        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        (IEnumerable<CompanyUserOutput> output, int count) = await _getCompanyUserByCompanyId.Execute(paginationInput, input, userIdAuth, companyId: input.CompanyId);

        return Ok(new { output, count });
    }

    [AllowAnonymous]
    [HttpGet("Verify/{token}")]
    public async Task<IActionResult> Verify(string token)
    {
        await _verify.Execute(token);

        EnvOutput env = _env.GetUrls();
        string url = $"{env.UrlFrontend}/{SystemConsts.Screens.CompanyUserVerified}";

        return Redirect(url);
    }

    [AuthorizeFilter]
    [HttpPut("UpdateCurrentMainCompanyUser")]
    public async Task<ActionResult> UpdateCurrentMainCompanyUser(Guid companyId)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _updateCurrentMainCompanyUser.Execute(userIdAuth, companyId);

        return NoContent();
    }

    [AuthorizeFilter]
    [HttpPut]
    public async Task<IActionResult> UpdateModules([FromBody] CompanyUserInput input)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _updateModuleCompanyUser.Execute(userIdAuth, input);

        return NoContent();
    }

    [AuthorizeFilter]
    [HttpPut("Disable")]
    public async Task<ActionResult> Disable(CompanyUserInput input)
    {
        if (input.CompanyId == Guid.Empty || input.UserId == Guid.Empty)
        {
            throw new ArgumentException($"Os parâmetros {nameof(input.CompanyId)} ou {nameof(input.UserId)} estão vazios.");
        }

        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _delete.Execute(userIdAuth, companyId: input.CompanyId, userId: input.UserId);

        return Ok(true);
    }
}