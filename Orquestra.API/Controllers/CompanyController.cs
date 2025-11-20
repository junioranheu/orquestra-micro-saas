using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Companies.Create;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.Companies.ResendVerifyEmail;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.Companies.Update;
using Orquestra.Application.UseCases.Companies.UpdatePlanType;
using Orquestra.Application.UseCases.Companies.Verify;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
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
        IUpdateCompany update,
        IVerifyCompany verify,
        IUpdatePlanTypeCompany updatePlanType,
        IResendVerifyEmailCompany resendVerifyEmailCompany
    ) : BaseController<CompanyController>
{
    private readonly IEnvService _env = env;
    private readonly ICreateCompany _create = create;
    private readonly IGetCompany _get = get;
    private readonly IUpdateCompany _update = update;
    private readonly IVerifyCompany _verify = verify;
    private readonly IUpdatePlanTypeCompany _updatePlanType = updatePlanType;
    private readonly IResendVerifyEmailCompany _resendVerifyEmailCompany = resendVerifyEmailCompany;

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

    [AuthorizeFilter]
    [HttpPut]
    public async Task<ActionResult> Update([FromForm] CompanyInput input)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        CompanyOutput output = await _update.Execute(userIdAuth, input);

        return Ok(output);
    }

    [AuthorizeFilter]
    [HttpPut("UpdatePlanType")]
    public async Task<ActionResult> UpdatePlanType([FromForm] CompanyInput input)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        CompanyInvoice? invoice = await _updatePlanType.Execute(userIdAuth, companyId: input.CompanyId.GetValueOrDefault(), planType: input.PlanType);

        return Ok(invoice);
    }

    [AuthorizeFilter(roles: [UserRoleEnum.Administrator, UserRoleEnum.Maintainer])]
    [HttpGet("GetAll")]
    public async Task<ActionResult> GetAll(bool onlyStatusTrue)
    {
        List<CompanyOutput>? output = await _get.Execute(onlyStatusTrue);

        return Ok(output);
    }

    [AuthorizeFilter]
    [HttpGet("GetAllByUserId")]
    public async Task<ActionResult> GetAllByUserId(Guid userId, bool onlyStatusTrue = true)
    {
        if (userId == Guid.Empty)
        {
            return NoContent();
        }

        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        (UserRoleEnum[] userRolesEnum, string[] _) = GetUserRolesAuth();

        if (userIdAuth != userId && !(userRolesEnum.Contains(UserRoleEnum.Administrator) || userRolesEnum.Contains(UserRoleEnum.Maintainer)))
        {
            throw new UnauthorizedAccessException("Você só pode visualizar a sua relação de empresas.");
        }

        List<CompanyOutput>? output = await _get.Execute(userId, onlyStatusTrue);

        return Ok(output);
    }

    [AllowAnonymous]
    [HttpGet("Verify/{token}")]
    public async Task<IActionResult> Verify(string token)
    {
        await _verify.Execute(token);

        EnvOutput env = _env.GetUrls();
        string url = $"{env.UrlFrontend}/{SystemConsts.Screens.CompanyVerified}";

        return Redirect(url);
    }

    [AuthorizeFilter]
    [HttpPost("ResendVerifyEmailCompany/{companyId}")]
    public async Task<IActionResult> ResendVerifyEmailCompany(Guid companyId)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _resendVerifyEmailCompany.Execute(userIdAuth, companyId);

        return Ok(true);
    }
}