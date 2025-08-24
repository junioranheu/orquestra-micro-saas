using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.CompanyUsers.CreateRange;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Application.UseCases.CompanyUsers.Verify;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.Infrastructure.Services.Env.Models;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyUserController(
        IEnvService env,
        ICreateRangeCompanyUser createRange,
        IGetCompanyUserByCompanyId getCompanyUserByCompanyId,
        IVerifyCompanyUser verify
    ) : BaseController<CompanyUserController>
{
    private readonly IEnvService _env = env;
    private readonly ICreateRangeCompanyUser _createRange = createRange;
    private readonly IGetCompanyUserByCompanyId _getCompanyUserByCompanyId = getCompanyUserByCompanyId;
    private readonly IVerifyCompanyUser _verify = verify;

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
}