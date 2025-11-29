using Orquestra.Domain.Consts;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Infrastructure.Services.PDF;

public sealed class PDFService : IPDFService
{
    private static readonly string MAIN_COLOR = Colors.Green.Darken4;
    private static readonly string SECONDARY_COLOR = Colors.Grey.Darken1;

    public byte[] GeneratePdfFromModel<T>(T model, Action<IContainer, T> buildContent, string titleDocument, bool addSignatureSection, bool showPageCounter, byte[]? logoBytes = null)
    {
        ArgumentNullException.ThrowIfNull(buildContent);

        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");

        Document document = Document.Create(container =>
        {
            container.Page(page =>
            {
                // Configurações globais da página;
                page.Size(PageSizes.A4);
                page.Margin(30); 
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial)); 

                // Header;
                page.Header().Element(container => ComposeHeader(container, titleDocument, logoBytes));

                // Conteúdo principal;
                page.Content().PaddingVertical(20).Column(col =>
                {
                    // Renderiza o conteúdo dinâmico passado por parâmetro;
                    col.Item().Element(x => buildContent(x, model));

                    // Seção de assinaturas;
                    if (addSignatureSection)
                    {
                        col.Item().PaddingTop(30).Element(ComposeSignatures);
                    }
                });

                // Footer
                page.Footer().Element(container => ComposeFooter(container, showPageCounter));
            });
        });

        return document.GeneratePdf();
    }

    private static void ComposeHeader(IContainer container, string title, byte[]? logoBytes)
    {
        container.Row(row =>
        {
            // Coluna da esquerda: logo;
            if (logoBytes is not null && logoBytes.Length > 0)
            {
                row.ConstantItem(80).Height(60).Element(img =>
                {
                    img.Image(logoBytes).FitArea();
                });
            }

            // Espaçador
            row.RelativeItem();

            // Coluna da direita: título e data;
            row.ConstantItem(250).Column(col =>
            {
                col.Item().AlignRight().Text(title).FontSize(18).SemiBold().FontColor(MAIN_COLOR);

                col.Item().AlignRight().Text(text =>
                {
                    text.Span("Emitido em: ").FontColor(SECONDARY_COLOR);
                    text.Span(GetDate().ToString("dd/MM/yyyy")).SemiBold();
                });

                // Linha decorativa abaixo do título
                col.Item().PaddingTop(5).BorderBottom(2).BorderColor(MAIN_COLOR);
            });
        });
    }

    private static void ComposeSignatures(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().PaddingBottom(15).Text("Autorização").FontSize(14).SemiBold().FontColor(MAIN_COLOR);

            col.Item().Row(row =>
            {
                // Assinatura 1;
                row.RelativeItem().Column(c =>
                {
                    c.Item().Height(40).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                    c.Item().PaddingTop(5).AlignCenter().Text("Assinatura do cliente").FontSize(9).FontColor(SECONDARY_COLOR);
                });

                // Espaço entre as assinaturas;
                row.ConstantItem(30);

                // Assinatura 2;
                row.RelativeItem().Column(c =>
                {
                    c.Item().Height(40).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                    c.Item().PaddingTop(5).AlignCenter().Text("Responsável").FontSize(9).FontColor(SECONDARY_COLOR);
                });
            });
        });
    }

    private static void ComposeFooter(IContainer container, bool showPageCounter)
    {
        container.Column(col =>
        {
            // Linha separadora do rodapé;
            col.Item().PaddingBottom(5).BorderTop(1).BorderColor(Colors.Grey.Lighten2);

            col.Item().Row(row =>
            {
                // Texto de Copyright ou Sistema
                row.RelativeItem().Text(text =>
                {
                    text.Span($"Gerado automaticamente pelo sistema {SystemConsts.App.NameApp}.").FontSize(8).FontColor(Colors.Grey.Medium);
                });

                // Contador de páginas;
                if (showPageCounter)
                {
                    row.RelativeItem().AlignRight().Text(text =>
                    {
                        text.Span("Página ").FontSize(8).FontColor(Colors.Grey.Medium);
                        text.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                        text.Span(" de ").FontSize(8).FontColor(Colors.Grey.Medium);
                        text.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                }
            });
        });
    }
}