using Mapster;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Quotes.Base;
using Orquestra.Application.UseCases.Quotes.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Quotes.Create;

public sealed class CreateQuote(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : QuoteBase(checkIfUserIsLinkedCompanyUser), ICreateQuote
{
    private readonly Context _context = context;

    public async Task Execute(Guid userIdAuth, QuoteInput input)
    {
        await Validate(input, userIdAuth);
        await Save(input);
    }

    #region extras
    private async Task Save(QuoteInput input)
    {
        var quote = input.Adapt<Quote>();

        if (input.Items is not null && input.Items.Count > 0)
        {
            foreach (var item in input.Items)
            {
                item.QuoteItemId = Guid.Empty;
            }
        }

        await _context.AddAsync(quote);
        await _context.SaveChangesAsync();

        //if (input.Items is not null && input.Items.Count > 0)
        //{
        //    foreach (var item in input.Items)
        //    {
        //        item.QuoteItemId = Guid.Empty;
        //    }

        //    await _context.AddRangeAsync(input.Items);
        //    await _context.SaveChangesAsync();
        //}
    }
    #endregion
}