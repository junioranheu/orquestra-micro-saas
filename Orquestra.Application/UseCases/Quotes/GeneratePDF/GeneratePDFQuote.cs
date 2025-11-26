using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Services.PDF;
using QuestPDF.Fluent;

namespace Orquestra.Application.UseCases.Quotes.GeneratePDF;

public sealed class GeneratePDFQuote(
        Context context, 
        ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser,
        IPDFService pdfService
    ) : IGeneratePDFQuote
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;
    private readonly IPDFService _pdfService = pdfService;

    public async Task<byte[]> Execute(Guid userIdAuth, Guid quoteId)
    {
        var quote = await _context.Quotes.
                    Include(x => x.Client).
                    AsNoTracking().
                    Where(x => x.QuoteId == quoteId && x.Status == true).
                    FirstOrDefaultAsync() ?? throw new ArgumentException(SystemConsts.Warnings.NotFoundData);

        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: quote.CompanyId, userId: userIdAuth, needCompanyAdmin: true);

        byte[] pdf = _pdfService.GeneratePdfFromModel(quote, (container, q) =>
        {
            container.Column(column =>
            {
                column.Item().Text(q.Title).FontSize(18).SemiBold();
                column.Item().Text($"Cliente: {q.Client?.FullName}").FontSize(12);

                column.Item().Element(inner =>
                {
                    foreach (var it in q.Items)
                    {
                        inner.Row(row =>
                        {
                            row.RelativeColumn().Text(it.Title);
                            row.ConstantColumn(60).AlignRight().Text(it.Quantity.ToString());
                            row.ConstantColumn(120).AlignRight().Text(it.UnitPrice.ToString("C"));
                            row.ConstantColumn(120).AlignRight().Text((it.UnitPrice * it.Quantity).ToString("C"));
                        });

                        inner.PaddingBottom(4);
                    }
                });

                column.Item().PaddingTop(8).Text($"Total: {q.Items.Sum(i => i.Quantity * i.UnitPrice):C}")
                      .FontSize(14).SemiBold().AlignRight();
            });
        });

        return pdf;
    }
}