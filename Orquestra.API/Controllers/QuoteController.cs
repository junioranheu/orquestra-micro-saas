using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Quotes.Create;
using Orquestra.Application.UseCases.Quotes.Delete;
using Orquestra.Application.UseCases.Quotes.GeneratePDF;
using Orquestra.Application.UseCases.Quotes.GetAllByCompanyId;
using Orquestra.Application.UseCases.Quotes.Shared;
using Orquestra.Application.UseCases.Quotes.Update;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Enums;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuoteController(
        IGetAllQuoteByCompanyId getQuoteByCompanyId,
        ICreateQuote create,
        IUpdateQuote update,
        IDeleteQuote delete,
        IGeneratePDFQuote pdf
    ) : BaseController<QuoteController>
{
    private readonly IGetAllQuoteByCompanyId _getQuoteByCompanyId = getQuoteByCompanyId;
    private readonly ICreateQuote _create = create;
    private readonly IUpdateQuote _update = update;
    private readonly IDeleteQuote _delete = delete;
    private readonly IGeneratePDFQuote _pdf = pdf;

    [AuthorizeFilter(modules: [ModuleEnum.Quote])]
    [HttpPost]
    public async Task<ActionResult> Create(QuoteInput input)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _create.Execute(userIdAuth, input);

        return Ok(true);
    }

    [AuthorizeFilter(modules: [ModuleEnum.Quote])]
    [HttpPut]
    public async Task<ActionResult> Update(QuoteInput input)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _update.Execute(userIdAuth, input);

        return Ok(true);
    }

    [AuthorizeFilter(modules: [ModuleEnum.Quote])]
    [HttpPut("Disable")]
    public async Task<ActionResult> Disable(Guid quoteId)
    {
        if (quoteId == Guid.Empty)
        {
            throw new ArgumentException($"O parâmetro {nameof(quoteId)} está vazio.");
        }

        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _delete.Execute(userIdAuth, quoteId);

        return Ok(true);
    }

    [AuthorizeFilter(modules: [ModuleEnum.Quote])]
    [HttpGet("GetAllByCompanyId")]
    public async Task<ActionResult> GetAllByCompanyId([FromQuery] PaginationInput paginationInput, [FromQuery] QuoteInput input)
    {
        if (input.CompanyId == Guid.Empty || input.CompanyId is null)
        {
            return NoContent();
        }

        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        (IEnumerable<QuoteOutput> output, int count) = await _getQuoteByCompanyId.Execute(paginationInput, input, userIdAuth, companyId: input.CompanyId.GetValueOrDefault());

        return Ok(new { output, count });
    }

    [AuthorizeFilter(modules: [ModuleEnum.Quote])]
    [HttpGet("GetPDF")]
    public async Task<IActionResult> GetQuotePdf([FromQuery] Guid quoteId)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        byte[] pdf = await _pdf.Execute(userIdAuth, quoteId);

        return File(pdf, "application/pdf", $"Orçamento - {quoteId}.pdf");
    }
}