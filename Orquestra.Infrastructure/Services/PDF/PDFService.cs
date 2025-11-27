using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Orquestra.Infrastructure.Services.PDF;

public sealed class PDFService : IPDFService
{
    public byte[] GeneratePdfFromModel<T>(T model, Action<IContainer, T> buildContent, string titleDocument, bool addSignatureSection, bool showPageCounter, byte[]? logoBytes = null)
    {
        ArgumentNullException.ThrowIfNull(buildContent);

        Document document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(25);
                page.DefaultTextStyle(x => x.FontSize(11));

                // Header;
                page.Header().Element(header =>
                {
                    header.Row(row =>
                    {
                        // Logo;
                        if (logoBytes is not null && logoBytes.Length > 0)
                        {
                            row.ConstantItem(70).Height(50).Element(img =>
                            {
                                img.Image(logoBytes).FitArea();
                            });
                        }

                        // Título;
                        row.RelativeItem().Element(title =>
                        {
                            title.AlignCenter().PaddingTop(6).Text(t =>
                            {
                                t.Span(titleDocument).FontSize(16).Bold();
                            });
                        });

                        // Data;
                        row.ConstantItem(120).Element(date =>
                        {
                            date.AlignRight().PaddingTop(6).Text(t =>
                            {
                                t.Span(DateTime.Now.ToString("dd/MM/yyyy")).FontSize(10);
                            });
                        });
                    });
                });

                // Conteúdo;
                page.Content().PaddingVertical(12).Column(col =>
                {
                    // Linha separadora;
                    col.Item().Element(x =>
                    {
                        x.PaddingBottom(6).BorderBottom(1).BorderColor(Colors.Grey.Medium);
                    });

                    // Conteúdo dinâmico do documento;
                    col.Item().Element(x => buildContent(x, model));

                    // Seção de assinatura opcional;
                    if (addSignatureSection)
                    {
                        col.Item().Element(sig =>
                        {
                            sig.PaddingTop(24).Column(signatureCol =>
                            {
                                signatureCol.Item().PaddingBottom(8).Text(t =>
                                {
                                    t.Span("Assinaturas").FontSize(13).Bold();
                                });

                                signatureCol.Item().Row(row =>
                                {
                                    row.RelativeItem().Element(c =>
                                    {
                                        c.PaddingTop(24).BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(16).Text("Assinatura do cliente").FontSize(10);
                                    });

                                    row.ConstantItem(24).Element(_ => { });

                                    row.RelativeItem().Element(c =>
                                    {
                                        c.PaddingTop(24).BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(16).Text("Assinatura do responsável").FontSize(10);
                                    });
                                });
                            });
                        });
                    }
                });

                // Contador;
                if (showPageCounter)
                {
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
                }
            });
        });

        return document.GeneratePdf();
    }
}