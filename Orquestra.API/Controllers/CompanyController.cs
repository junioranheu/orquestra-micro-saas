using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Companies.Create;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.Companies.GetModule;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.Companies.UpdateModule;
using Orquestra.Application.UseCases.Companies.Verify;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.Infrastructure.Services.Env.Models;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController(
        IEnvService env,
        ICreateCompany create,
        IGetCompany get,
        IVerifyCompany verify,
        IGetModuleCompany getModuleCompany,
        IUpdateModuleCompany updateModuleCompany
    ) : BaseController<CompanyController>
{
    private readonly IEnvService _env = env;
    private readonly ICreateCompany _create = create;
    private readonly IGetCompany _get = get;
    private readonly IVerifyCompany _verify = verify;
    private readonly IGetModuleCompany _getModuleCompany = getModuleCompany;
    private readonly IUpdateModuleCompany _updateModuleCompany = updateModuleCompany;

    [AuthorizeFilter]
    [HttpPost]
    public async Task<ActionResult> Create([FromForm] CompanyInput input)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        CompanyOutput output = await _create.Execute(userIdAuth, input);

        return Ok(output);
    }

    [AuthorizeFilter]
    [HttpGet]
    public async Task<ActionResult> Get(Guid companyId)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        CompanyOutput? output = await _get.Execute(userIdAuth, companyId);

        return Ok(output);
    }

    [AuthorizeFilter(UserRoleEnum.Administrator, UserRoleEnum.Maintainer)]
    [HttpGet("GetAll")]
    public async Task<ActionResult> GetAll()
    {
        List<CompanyOutput>? output = await _get.Execute();

        return Ok(output);
    }

    [AuthorizeFilter]
    [HttpGet("GetAllByUserId")]
    public async Task<ActionResult> GetAllByUserId(Guid userId)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        (UserRoleEnum[] userRolesEnum, string[] _) = GetUserRolesAuth();

        if (userIdAuth != userId && !(userRolesEnum.Contains(UserRoleEnum.Administrator) || userRolesEnum.Contains(UserRoleEnum.Maintainer)))
        {
            throw new UnauthorizedAccessException("Você só pode visualizar a sua relação de empresas.");
        }

        List<CompanyOutput>? output = await _get.Execute(userId);

        return Ok(output);
    }

    [AllowAnonymous]
    [HttpGet("Verify/{token}")]
    public async Task<IActionResult> Verify(string token)
    {
        await _verify.Execute(token);

        EnvOutput env = _env.GetUrls();
        string url = $"{env.UrlFrontend}/{SystemConsts.ScreenCompanyHasBeenVerified}";

        return Redirect(url);
    }

    [AuthorizeFilter]
    [HttpGet("GetModules")]
    public async Task<IActionResult> GetModules(Guid companyId)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        CompanyModulesOutput output = await _getModuleCompany.Execute(userIdAuth, companyId);

        return Ok(output);
    }

    [AuthorizeFilter]
    [HttpPut("UpdateModules")]
    public async Task<IActionResult> UpdateModules([FromBody] CompanyUpdateModuleInput input)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _updateModuleCompany.Execute(userIdAuth, input);

        return NoContent();
    }
}