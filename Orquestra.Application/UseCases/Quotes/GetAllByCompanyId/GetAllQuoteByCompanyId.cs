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

    public async Task<(IEnumerable<QuoteOutput> output, int count)> Execute(PaginationInput pagination, QuoteInput input, Guid userIdAuth, Guid companyId)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: false);

        var query = _context.Quotes.
                    Include(x => x.Client).
                    Include(x => x.Company).
                    AsNoTracking().
                    Where(x =>
                        x.CompanyId == companyId &&
                        ((input.QuoteId == null || input.QuoteId == Guid.Empty) || x.QuoteId == input.QuoteId) &&
                        (string.IsNullOrEmpty(input.Title) || x.Title!.ToLower().Contains(input.Title.ToLower())) &&
                        (string.IsNullOrEmpty(input.Observation) || x.Observation!.ToLower().Contains(input.Observation!.ToLower())) &&
                        (input.ValidUntil == null || input.ValidUntil == DateTime.MinValue || (x.ValidUntil >= input.ValidUntil.Value.Date && x.ValidUntil < input.ValidUntil.Value.Date.AddDays(1))) &&
                        x.Status == true
                    ).
                    OrderByDescending(x => x.LastModificationDate ?? DateTime.MinValue).
                    ThenByDescending(x => x.CreatedDate);

        (IEnumerable<Quote> result, int count) = await PagedQuery.Execute(query, pagination);

        var output = result.Adapt<List<QuoteOutput>>();

        output = await PopulateItems(output);

        return (output, count);
    }

    #region extras
    private async Task<List<QuoteOutput>> PopulateItems(List<QuoteOutput> quotes)
    {
        if (quotes.Count == 0)
        {
            return quotes;
        }

        List<Guid> ids = quotes.Where(q => q.QuoteId != Guid.Empty).Select(q => q.QuoteId).Distinct().ToList();

        if (ids.Count == 0)
        {
            return quotes;
        }

        List<QuoteItem> items = await _context.QuoteItems.AsNoTracking().Where(i => ids.Contains(i.QuoteId)).ToListAsync();
        Dictionary<Guid, List<QuoteItem>> grouped = items.GroupBy(i => i.QuoteId).ToDictionary(g => g.Key, g => g.ToList());

        foreach (var q in quotes)
        {
            if (grouped.TryGetValue(q.QuoteId, out var list))
            {
                q.Items = list.Adapt<List<QuoteItem>>();
            }
            else
            {
                q.Items = [];
            }
        }

        return quotes;
    }
    #endregion
}