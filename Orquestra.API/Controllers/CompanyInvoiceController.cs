using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.CompanyInvoices.Get;
using Orquestra.Domain.Enums;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyInvoiceController(IGetCompanyInvoice get) : BaseController<CompanyInvoiceController>
{
    private readonly IGetCompanyInvoice _get = get;

    [AuthorizeFilter]
    [HttpGet]
    public async Task<ActionResult> Get(Guid companyId, CompanyInvoiceSituationEnum? companyInvoiceSituationEnum)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        var output = await _get.Execute(userIdAuth, companyId, companyInvoiceSituationEnum);

        return Ok(output);
    }
}