using Orquestra.Application.UseCases.Quotes.Shared;
using Orquestra.Application.UseCases.Shared;

namespace Orquestra.Application.UseCases.Quotes.GetAllByCompanyId;

public interface IGetAllQuoteByCompanyId
{
    Task<(IEnumerable<QuoteOutput> output, int count)> Execute(PaginationInput pagination, QuoteInput input, Guid userIdAuth, Guid companyId);
}