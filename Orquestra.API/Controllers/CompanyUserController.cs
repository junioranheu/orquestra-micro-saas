using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.CompanyUsers.Invite;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Application.UseCases.CompanyUsers.UpdateCurrentMainCompany;
using Orquestra.Application.UseCases.CompanyUsers.Verify;
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
        IUpdateCurrentMainCompanyUser updateCurrentMainCompanyUser
    ) : BaseController<CompanyUserController>
{
    private readonly IEnvService _env = env;
    private readonly IInviteCompanyUser _invite = invite;
    private readonly IGetCompanyUserByCompanyId _getCompanyUserByCompanyId = getCompanyUserByCompanyId;
    private readonly IVerifyCompanyUser _verify = verify;
    private readonly IUpdateCurrentMainCompanyUser _updateCurrentMainCompanyUser = updateCurrentMainCompanyUser;

    [AuthorizeFilter]
    [HttpPost("InviteUser")]
    public async Task<ActionResult> InviteUser(string email)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        CompanyUserOutput output = await _invite.Execute(userIdAuth, email);

        return Ok(output);
    }

    [AuthorizeFilter]
    [HttpPost("CreateRange")]
    public async Task<ActionResult> CreateRange([FromBody] List<CompanyUserInput> input)
    {
        if (input.Count == 0)
        {
            throw new ArgumentException($"A lista de usuários não pode estar vazia.");
        }

        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        List<CompanyUserOutput> output = await _createRange.Execute(userIdAuth, input);

        return Ok(output);
    }

    [AuthorizeFilter]
    [HttpGet("GetAllByCompanyId")]
    public async Task<ActionResult> GetAllByCompanyId(Guid companyId)
    {
        if (companyId == Guid.Empty)
        {
            throw new Exception($"O parâmetro {nameof(companyId)} não pode estar vazio.");
        }

        List<CompanyUserOutput>? output = await _getCompanyUserByCompanyId.Execute(companyId);

        return Ok(output);
    }

    [AllowAnonymous]
    [HttpGet("Verify/{token}")]
    public async Task<IActionResult> Verify(string token)
    {
        await _verify.Execute(token);

        EnvOutput env = _env.GetUrls();
        string url = $"{env.UrlFrontend}/{SystemConsts.ScreenCompanyUserHasBeenVerified}";

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
}

