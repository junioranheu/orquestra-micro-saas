using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.CompanyInvoices.Get;
using Orquestra.Application.UseCases.CompanyInvoices.Pay;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyInvoiceController(IGetCompanyInvoice get, IPayCompanyInvoice pay) : BaseController<CompanyInvoiceController>
{
    private readonly IGetCompanyInvoice _get = get;
    private readonly IPayCompanyInvoice _pay = pay;

    [AuthorizeFilter]
    [HttpGet]
    public async Task<ActionResult> Get([FromQuery] PaginationInput paginationInput, Guid companyId, CompanyInvoiceSituationEnum? companyInvoiceSituationEnum)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        (IEnumerable<CompanyInvoice> output, int count) = await _get.Execute(paginationInput, userIdAuth, companyId, companyInvoiceSituationEnum);

        return Ok(new { output, count });
    }

    [AllowAnonymous]
    [HttpPut("Pay/{companyInvoiceId}")]
    public async Task<IActionResult> Pay(Guid companyInvoiceId)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _pay.Execute(userIdAuth, companyInvoiceId);

        return Ok(true);
    }
}