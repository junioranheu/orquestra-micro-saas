namespace Orquestra.Application.UseCases.Quotes.GeneratePDF;

public interface IGeneratePDFQuote
{
    Task<byte[]> Execute(Guid userIdAuth, Guid quoteId);
}