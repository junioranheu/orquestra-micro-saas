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
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using static Orquestra.Utils.Fixtures.Get;

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

    private static readonly string MAIN_COLOR = Colors.Green.Darken2;

    public async Task<(byte[] pdf, string title)> Execute(Guid userIdAuth, Guid quoteId)
    {
        Quote quote = await GetQuote(userIdAuth, quoteId);

        if (quote.ValidUntil is not null && quote.ValidUntil != DateTime.MinValue)
        {
            if (quote.ValidUntil.GetValueOrDefault().Date < ConvertToBrasiliaTime(GetDate()).Date)
            {
                throw new ArgumentException("Não é possível gerar um documento com a data de validade expirada.");
            }
        }
   
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: quote.CompanyId, userId: userIdAuth, needCompanyAdmin: true);

        string title = $"Orçamento: {quote.Company?.Name} • {GetFirstWord(quote.Client?.FullName)}";
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

    private void BuildQuoteContent(IContainer container, Quote input)
    {
        container.Column(col =>
        {
            // 1. Cabeçalho do conteúdo: dados do cliente e validade;
            col.Item().Element(c =>
            {
                c.Background(Colors.Grey.Lighten4).
                  Border(1).
                  BorderColor(Colors.Grey.Lighten2).
                  Padding(10).
                  Row(row =>
                  {
                      // Lado esquerdo: cliente;
                      row.RelativeItem().Column(info =>
                      {
                          info.Item().Text("Cliente").FontSize(9).FontColor(Colors.Grey.Medium);
                          info.Item().Text(input.Client?.FullName ?? "N/A").FontSize(11).SemiBold();
                      });

                      // Lado direito: validade e título;
                      row.RelativeItem().Column(info =>
                      {
                          info.Item().AlignRight().Text("Validade da proposta").FontSize(9).FontColor(Colors.Grey.Medium);
                          info.Item().AlignRight().Text($"{input.ValidUntil:dd/MM/yyyy}").FontSize(11).SemiBold();
                      });
                  });
            });

            col.Spacing(20);

            // 2. Tabela de Itens
            col.Item().Table(table =>
            {
                // Definição das colunas;
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(30); // # (Índice);
                    columns.RelativeColumn(4); // Descrição;
                    columns.RelativeColumn(1); // Qtd;
                    columns.RelativeColumn(2); // Valor unitário;
                    columns.RelativeColumn(2); // Total;
                });

                // Cabeçalho da tabela;
                table.Header(header =>
                {
                    IContainer HeaderStyle(IContainer x) => x.Background(MAIN_COLOR).PaddingVertical(5).PaddingHorizontal(5);

                    header.Cell().Element(HeaderStyle).Text("#").FontColor(Colors.White).FontSize(10);
                    header.Cell().Element(HeaderStyle).Text("Descrição").FontColor(Colors.White).FontSize(10);
                    header.Cell().Element(HeaderStyle).AlignRight().Text("Qtd").FontColor(Colors.White).FontSize(10); // Alinhado à direita;
                    header.Cell().Element(HeaderStyle).AlignRight().Text("Unitário").FontColor(Colors.White).FontSize(10);
                    header.Cell().Element(HeaderStyle).AlignRight().Text("Total").FontColor(Colors.White).FontSize(10);
                });

                // Linhas da tabela;
                for (int i = 0; i < input.Items.Count; i++)
                {
                    QuoteItem item = input.Items[i];
                    int index = i + 1;

                    // Efeito zebra: linhas pares brancas, ímpares cinza clarinho;
                    Color backgroundColor = index % 2 == 0 ? Colors.White : Colors.Grey.Lighten5;
                    IContainer CellStyle(IContainer c) => c.Background(backgroundColor).BorderBottom(1).BorderColor(Colors.Grey.Lighten3).PaddingVertical(5).PaddingHorizontal(5);

                    table.Cell().Element(CellStyle).Text($"{index}").FontSize(10).FontColor(Colors.Grey.Darken2);
                    table.Cell().Element(CellStyle).Text(item.Title).FontSize(10).FontColor(Colors.Grey.Darken2);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.Quantity}").FontSize(10).FontColor(Colors.Grey.Darken2);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.UnitPrice:C2}").FontSize(10).FontColor(Colors.Grey.Darken2);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.TotalPrice:C2}").FontSize(10).FontColor(Colors.Grey.Darken2);
                }
            });

            col.Spacing(10);

            // 3. Seção de totais (alinhada à direita);
            col.Item().Row(row =>
            {
                // Espaço vazio na esquerda para empurrar o total para a direita;
                row.RelativeItem();

                // Bloco de totais;
                row.ConstantItem(200).Column(totalCol =>
                {
                    // Linha final do total;
                    totalCol.Item().Background(Colors.Grey.Lighten4).Padding(10).Row(r =>
                    {
                        decimal grandTotal = input.Items.Sum(x => x.TotalPrice);

                        r.RelativeItem().AlignLeft().Text("TOTAL").FontSize(12).SemiBold().FontColor(MAIN_COLOR);
                        r.RelativeItem().AlignRight().Text($"{grandTotal:C2}").FontSize(12).Bold().FontColor(MAIN_COLOR);
                    });
                });
            });

            if (!string.IsNullOrEmpty(input.Observation))
            {
                col.Spacing(20);

                col.Item().PaddingTop(10).BorderTop(1).BorderColor(Colors.Grey.Lighten2).Column(obs =>
                {
                    obs.Item().PaddingTop(10).Text("Observações:").FontSize(10).Bold();
                    obs.Item().Text(input.Observation).FontSize(9).FontColor(Colors.Grey.Medium);
                });
            }
        });
    }
    #endregion
}