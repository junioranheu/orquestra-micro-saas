using Orquestra.Application.UseCases.Quotes.Shared;
using Orquestra.Application.UseCases.Shared;

namespace Orquestra.Application.UseCases.Quotes.GetAllByCompanyId;

public interface IGetAllQuoteByCompanyId
{
    Task<(IEnumerable<QuoteOutput> output, int count)> Execute(PaginationInput pagination, Guid userIdAuth, Guid companyId, QuoteInput input);
}