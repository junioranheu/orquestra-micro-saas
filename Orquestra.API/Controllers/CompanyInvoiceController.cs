using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.CompanyInvoices.Get;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.Infrastructure.Services.Env.Models;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyInvoiceController(IEnvService env, IGetCompanyInvoice get) : BaseController<CompanyInvoiceController>
{
    private readonly IEnvService _env = env;
    private readonly IGetCompanyInvoice _get = get;

    [AuthorizeFilter]
    [HttpGet]
    public async Task<ActionResult> Get(Guid companyId, CompanyInvoiceSituationEnum? companyInvoiceSituationEnum)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        var output = await _get.Execute(userIdAuth, companyId, companyInvoiceSituationEnum);

        return Ok(output);
    }

    [AllowAnonymous]
    [HttpGet("Pay/{invoiceNumber}")]
    public async Task<IActionResult> Pay(string invoiceNumber)
    {
        // TO DO;
        // await _pay.Execute(invoiceNumber);

        EnvOutput env = _env.GetUrls();
        string url = $"{env.UrlFrontend}/{SystemConsts.ScreenDashboard}";

        return Redirect(url);
    }
}