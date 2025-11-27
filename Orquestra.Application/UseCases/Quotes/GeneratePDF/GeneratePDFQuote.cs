using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Quotes.GetAllByCompanyId;
using Orquestra.Application.UseCases.Quotes.Shared;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Services.PDF;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Orquestra.Application.UseCases.Quotes.GeneratePDF;

public sealed class GeneratePDFQuote(
        Context context,
        IGetAllQuoteByCompanyId getAllQuoteByCompanyId,
        ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser,
        IPDFService pdfService
    ) : IGeneratePDFQuote
{
    private readonly Context _context = context;
    private readonly IGetAllQuoteByCompanyId _getAllQuoteByCompanyId = getAllQuoteByCompanyId;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;
    private readonly IPDFService _pdfService = pdfService;

    public async Task<(byte[] pdf, string title)> Execute(Guid userIdAuth, Guid quoteId)
    {
        Quote quote = await GetQuote(userIdAuth, quoteId);

        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: quote.CompanyId, userId: userIdAuth, needCompanyAdmin: true);

        string title = $"{quote.Company?.Name} • Orçamento • {quote.Client?.FullName}";
        byte[] pdf = _pdfService.GeneratePdfFromModel(model: quote, buildContent: BuildQuoteContent, titleDocument: title, addSignatureSection: true, showPageCounter: false);

        return (pdf, title);
    }

    #region extras
    private async Task<Quote> GetQuote(Guid userIdAuth, Guid quoteId)
    {
        PaginationInput pagination = new()
        {
            Index = 0,
            Limit = 1
        };

        QuoteInput input = new()
        {
            QuoteId = quoteId
        };

        Guid? companyId = await _context.Quotes.AsNoTracking().Where(x => x.QuoteId == quoteId).Select(x => x.CompanyId).FirstOrDefaultAsync();

        if (companyId is null || companyId == Guid.Empty)
        {
            throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundData);
        }

        (IEnumerable<QuoteOutput> output, int _) = await _getAllQuoteByCompanyId.Execute(pagination, input, userIdAuth, companyId.GetValueOrDefault());

        if (!output.Any())
        {
            throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundData);
        }

        QuoteOutput? outputFirst = output.FirstOrDefault();
        Quote model = outputFirst.Adapt<Quote>();

        return model;
    }

    private static void BuildQuoteContent(IContainer container, Quote input)
    {
        container.Column(col =>
        {
            // Título do orçamento;
            col.Item().Text(input.Title ?? "Orçamento").FontSize(16).Bold();

            col.Spacing(8);

            // Dados do cliente;
            col.Item().Text($"Cliente: {input.Client?.FullName}").FontSize(12);
            col.Item().Text($"Validade: {input.ValidUntil:dd/MM/yyyy}").FontSize(12);

            col.Spacing(12);

            // Itens;
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(4); // Título;
                    cols.RelativeColumn(1); // Quantidade;
                    cols.RelativeColumn(2); // Preço unitário;
                    cols.RelativeColumn(2); // Total;
                });

                // Cabeçalho;
                table.Header(header =>
                {
                    header.Cell().Text("Item").Bold();
                    header.Cell().Text("Qtd").Bold();
                    header.Cell().Text("Valor").Bold();
                    header.Cell().Text("Total").Bold();
                });

                // Linhas;
                foreach (var item in input.Items)
                {
                    table.Cell().Text(item.Title);
                    table.Cell().Text(item.Quantity.ToString());
                    table.Cell().Text(item.UnitPrice.ToString("C2"));
                    table.Cell().Text(item.TotalPrice.ToString("C2"));
                }
            });    

            col.Spacing(12);

            decimal grandTotal = input.Items.Sum(x => x.TotalPrice);
            col.Item().Text($"Total: {grandTotal:C2}").FontSize(14).Bold();
        });
    }
    #endregion
}