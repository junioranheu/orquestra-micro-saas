using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Quotes.Shared;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Quotes.GetAllByCompanyId;

public sealed class GetAllQuoteByCompanyId(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IGetAllQuoteByCompanyId
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task<(IEnumerable<QuoteOutput> output, int count)> Execute(PaginationInput pagination, Guid userIdAuth, Guid companyId, QuoteInput input)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: false);

        var query = _context.Quotes.
                    AsNoTracking().
                    Where(x =>
                        x.CompanyId == companyId &&
                        x.Status == true &&
                        (string.IsNullOrEmpty(input.Title) || x.Title!.ToLower().Contains(input.Title.ToLower())) &&
                        (string.IsNullOrEmpty(input.Observation) || x.Observation!.ToLower().Contains(input.Observation!.ToLower()))
                    ).
                    OrderByDescending(x => x.LastModificationDate ?? DateTime.MinValue).
                    ThenByDescending(x => x.CreatedDate);

        (IEnumerable<Quote> result, int count) = await PagedQuery.Execute(query, pagination);

        var output = result.Adapt<List<QuoteOutput>>();

        return (output, count);
    }
}