using Microsoft.EntityFrameworkCore;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.CompanyInvoices.Pay;

public sealed class PayCompanyInvoice(Context context) : IPayCompanyInvoice
{
    private readonly Context _context = context;

    public async Task Execute(Guid userIdAuth, Guid companyInvoiceId)
    {
        var invoice = await _context.CompanyInvoices.
                      // AsNoTracking(). // Propositalmente sem AsNoTracking;
                      Where(x => x.CompanyInvoiceId == companyInvoiceId).
                      FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundCompanyInvoice);

        // TO DO: Efetuar pagamento (?);

        await UpdateCompanyData(companyId: invoice.CompanyId);
        await UpdateCompanyInvoiceData(invoice);
    }

    #region extras
    private async Task UpdateCompanyData(Guid companyId)
    {
        var company = await _context.Companies.
                      // AsNoTracking(). // Propositalmente sem AsNoTracking; 
                      Where(x => x.CompanyId == companyId).
                      FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundCompanyInvoice);

        company.CompanySituation = CompanySituationEnum.Approved;
        company.PlanStartDate = GetDate();
        company.PlanEndDate = GetDate().AddDays(SystemConsts.Time.PlanDurationDays);

        _context.Update(company);
        await _context.SaveChangesAsync();
    }

    private async Task UpdateCompanyInvoiceData(CompanyInvoice invoice)
    {
        invoice.CompanyInvoiceSituation = CompanyInvoiceSituationEnum.Paid;

        _context.Update(invoice);
        await _context.SaveChangesAsync();
    }
    #endregion
}