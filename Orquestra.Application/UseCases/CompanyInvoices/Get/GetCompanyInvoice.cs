using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.CompanyInvoices.Get;

public sealed class GetCompanyInvoice(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IGetCompanyInvoice
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task<CompanyInvoice> Execute(Guid userIdAuth, Guid companyInvoiceId)
    {
        var result = await _context.CompanyInvoices.
                     AsNoTracking().
                     Where(x => x.CompanyInvoiceId == companyInvoiceId && x.Status == true).
                     FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundCompanyInvoice);

        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: result.CompanyId, userId: userIdAuth, needCompanyAdmin: true);

        return result;
    }

    public async Task<(IEnumerable<CompanyInvoice> output, int count)> Execute(PaginationInput pagination, Guid userIdAuth, Guid companyId, CompanyInvoiceSituationEnum? companyInvoiceSituationEnum)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: true);

        var query = _context.CompanyInvoices.
                    AsNoTracking().
                    Where(x =>
                        x.CompanyId == companyId &&
                        x.Status == true &&
                        (companyInvoiceSituationEnum == null || x.CompanyInvoiceSituation == companyInvoiceSituationEnum)
                    ).OrderByDescending(x => x.CreatedDate);

        (IEnumerable<CompanyInvoice> output, int count) = await PagedQuery.Execute(query, pagination);

        return (output, count);
    }
}