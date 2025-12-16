using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Quotes.Delete;

public sealed class DeleteQuote(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IDeleteQuote
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task Execute(Guid userIdAuth, Guid quoteId)
    {
        Quote? quote = await _context.Quotes.
                       // AsNoTracking(). // Propositalmente sem AsNoTracking;
                       Where(x => x.QuoteId == quoteId).
                       FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundData);

        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: quote.CompanyId, userId: userIdAuth, needCompanyAdmin: true);

        await RemoveQuoteItems(quoteId: quote.QuoteId);

        quote.Status = false;
        _context.Update(quote);
        await _context.SaveChangesAsync();
    }

    #region extras
    private async Task RemoveQuoteItems(Guid quoteId)
    {
        var items = await _context.QuoteItems.
                    // AsNoTracking(). // Propositalmente sem AsNoTracking;
                    Where(x => x.QuoteId == quoteId).
                    ToListAsync();

        if (items is not null && items.Count > 0)
        {
            foreach (var item in items)
            {
                item.Status = false;
            }

            _context.UpdateRange(items);
            await _context.SaveChangesAsync();
        }
    }
    #endregion
}