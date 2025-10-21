using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.CompanyInvoices.Get;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
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
    public async Task<ActionResult> Get([FromQuery] PaginationInput paginationInput, Guid companyId, CompanyInvoiceSituationEnum? companyInvoiceSituationEnum)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        (IEnumerable<CompanyInvoice> output, int count) = await _get.Execute(paginationInput, userIdAuth, companyId, companyInvoiceSituationEnum);

        return Ok(new { output, count });
    }

    [AllowAnonymous]
    [HttpPost("Pay/{invoiceNumber}")]
    public async Task<IActionResult> Pay(string invoiceNumber)
    {
        // TO DO;
        // await _pay.Execute(invoiceNumber);

        EnvOutput env = _env.GetUrls();
        string url = $"{env.UrlFrontend}/{SystemConsts.Screens.Dashboard}";

        return Redirect(url);
    }
}