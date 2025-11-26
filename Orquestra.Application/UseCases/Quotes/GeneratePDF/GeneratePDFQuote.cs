using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Quotes.GeneratePDF;

public sealed class GeneratePDFQuote(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IGeneratePDFQuote
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task<byte[]> Execute(Guid userIdAuth, Guid quoteId)
    {
        var quote = await _context.Quotes.
                    AsNoTracking().
                    Where(x => x.QuoteId == quoteId && x.Status == true).
                    FirstOrDefaultAsync() ?? throw new ArgumentException(SystemConsts.Warnings.NotFoundData);

        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: quote.CompanyId, userId: userIdAuth, needCompanyAdmin: true);

        throw new NotImplementedException();
    }
}