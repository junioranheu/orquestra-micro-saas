using Orquestra.Application.UseCases.Quotes.Shared;

namespace Orquestra.Application.UseCases.Quotes.Update;

public interface IUpdateQuote
{
    Task Execute(Guid userIdAuth, QuoteInput input);
}