namespace Orquestra.Application.UseCases.Quotes.GeneratePDF;

public interface IGeneratePDFQuote
{
    Task<(byte[] pdf, string title)> Execute(Guid userIdAuth, Guid quoteId);
}