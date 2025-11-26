using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Quotes.Base;
using Orquestra.Application.UseCases.Quotes.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Quotes.Update;

public sealed class UpdateQuote(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : QuoteBase(checkIfUserIsLinkedCompanyUser), IUpdateQuote
{
    private readonly Context _context = context;

    public async Task Execute(Guid userIdAuth, QuoteInput input)
    {
        Quote? quote = await _context.Quotes.
                       // AsNoTracking(). // Propositalmente sem AsNoTracking;
                       Where(x => x.QuoteId == input.QuoteId).
                       FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundData);

        await Validate(input, userIdAuth);
        await Update(input, quote);
    }

    #region extras
    private async Task Update(QuoteInput input, Quote quote)
    {
        quote.ClientId = input.ClientId.GetValueOrDefault();
        quote.Title = input.Title;
        quote.Observation = input.Observation;
        quote.ValidUntil = input.ValidUntil.GetValueOrDefault();
        quote.QuoteStatus = input.QuoteStatus.GetValueOrDefault();

        if (input.Items is not null && input.Items.Count > 0)
        {
            List<QuoteItem> previousItems = await _context.QuoteItems.Where(x => x.QuoteId == quote.QuoteId).ToListAsync();

            if (previousItems.Count != 0)
            {
                _context.RemoveRange(previousItems);
                await _context.SaveChangesAsync();
            }

            quote.Items.Clear();

            foreach (var item in input.Items)
            {
                item.QuoteItemId = Guid.Empty;
                item.QuoteId = quote.QuoteId;
                quote.Items.Add(item);
            }
        }

        _context.Update(quote);
        await _context.SaveChangesAsync();
    }
    #endregion
}