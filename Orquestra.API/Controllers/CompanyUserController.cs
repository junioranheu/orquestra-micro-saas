using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.CompanyUsers.Invite;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Application.UseCases.CompanyUsers.UpdateCurrentMain;
using Orquestra.Application.UseCases.CompanyUsers.UpdateModule;
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
        IUpdateModuleCompanyUser updateModuleCompanyUser
    ) : BaseController<CompanyUserController>
{
    private readonly IEnvService _env = env;
    private readonly IInviteCompanyUser _invite = invite;
    private readonly IGetCompanyUserByCompanyId _getCompanyUserByCompanyId = getCompanyUserByCompanyId;
    private readonly IVerifyCompanyUser _verify = verify;
    private readonly IUpdateCurrentMainCompanyUser _updateCurrentMainCompanyUser = updateCurrentMainCompanyUser;
    private readonly IUpdateModuleCompanyUser _updateModuleCompanyUser = updateModuleCompanyUser;

    [AuthorizeFilter]
    [HttpPost("InviteUser")]
    public async Task<ActionResult> InviteUser(Guid companyId, string email)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _invite.Execute(userIdAuth, companyId, email, isFirstAdministrator: false);

        return NoContent();
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
    [HttpPut("Module")]
    public async Task<IActionResult> UpdateModules([FromBody] CompanyUserUpdateModuleInput input)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _updateModuleCompanyUser.Execute(userIdAuth, input);

        return NoContent();
    }
}