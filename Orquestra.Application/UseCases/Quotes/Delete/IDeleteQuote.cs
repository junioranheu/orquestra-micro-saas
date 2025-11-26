
namespace Orquestra.Application.UseCases.Quotes.Delete
{
    public interface IDeleteQuote
    {
        Task Execute(Guid userIdAuth, Guid quoteId);
    }
}