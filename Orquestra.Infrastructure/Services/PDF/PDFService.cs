using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Orquestra.Infrastructure.Services.PDF;

public sealed class PDFService : IPDFService
{
    public PDFService() { }

    /// <summary>
    /// Gera um PDF em memória a partir de um modelo T;
    /// Recebe um 'builder' que define o conteúdo do PDF usando a API do QuestPDF;
    /// Retorna byte[] pronto pra salvar/retornar por API;
    /// </summary>
    public byte[] GeneratePdfFromModel<T>(T model, Action<IContainer, T> buildContent)
    {
        ArgumentNullException.ThrowIfNull(buildContent);

        Document document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(25);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().
                     AlignCenter().
                     Text("Documento gerado").
                     SemiBold().FontSize(14);

                page.Content().
                     PaddingVertical(10).
                     Element(x => buildContent(x, model));

                page.Footer().
                     AlignCenter().
                     Text(text =>
                     {
                         text.CurrentPageNumber();
                         text.Span(" / ");
                         text.TotalPages();
                     });
            });
        });

        return document.GeneratePdf();
    }
}