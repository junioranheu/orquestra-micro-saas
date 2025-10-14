using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.CompanyInvoices.Get;

public sealed class GetCompanyInvoice(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IGetCompanyInvoice
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task<CompanyInvoice> Execute(Guid userIdAuth, Guid companyId)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: true);

        var result = await _context.CompanyInvoices.
                     AsNoTracking().
                     Where(x => x.CompanyId == companyId).
                     FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundCompanyInvoice);

        return result;
    }

    public async Task<List<CompanyInvoice>> Execute(Guid userIdAuth, Guid companyId, CompanyInvoiceSituationEnum? companyInvoiceSituationEnum)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: true);

        var result = await _context.CompanyInvoices.
                     AsNoTracking().
                     Where(x => 
                        x.CompanyId == companyId && 
                        (companyInvoiceSituationEnum == null || x.CompanyInvoiceSituation == companyInvoiceSituationEnum)
                     ).ToListAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundCompanyInvoice);

        return result;
    }
}