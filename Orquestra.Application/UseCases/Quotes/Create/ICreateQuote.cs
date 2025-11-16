using Orquestra.Application.UseCases.Quotes.Shared;

namespace Orquestra.Application.UseCases.Quotes.Create;

public interface ICreateQuote
{
    Task Execute(Guid userIdAuth, QuoteInput input);
}