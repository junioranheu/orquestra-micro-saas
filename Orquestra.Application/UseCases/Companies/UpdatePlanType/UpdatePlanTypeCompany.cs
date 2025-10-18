using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyInvoices.Create;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Companies.UpdatePlanType;

public sealed class UpdatePlanTypeCompany(
        Context context,
        ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser,
        ICreateCompanyInvoice createCompanyInvoice
    ) : IUpdatePlanTypeCompany
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;
    private readonly ICreateCompanyInvoice _createCompanyInvoice = createCompanyInvoice;

    public async Task Execute(Guid userIdAuth, Guid companyId, PlanTypeEnum planType)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: true);

        var result = await _context.Companies.
                     // AsNoTracking(). // Propositalmente sem AsNoTracking;
                     Where(x => x.CompanyId == companyId).
                     FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundCompany);

        if (!result.Status)
        {
            throw new InvalidOperationException(SystemConsts.Warnings.NeedToVerifyCompany);
        }

        // #1 - Criar cobrança, obrigatoriamente antes de normalizar o input.Modules;
        await _createCompanyInvoice.Execute(userIdAuth, companyId, planType);

        // #2 - Atualizar os dados da empresa, em si;
        await UpdateCompanyData(company: result, planType);
    }

    #region extras
    private async Task UpdateCompanyData(Company company, PlanTypeEnum planType)
    {
        company.CompanySituation = CompanySituationEnum.PendingPayment;
        company.PlanStartDate = (company.PlanStartDate is null || company.PlanStartDate == DateTime.MinValue) ? GetDate() : company.PlanStartDate;
        company.PlanEndDate = (company.PlanStartDate is null || company.PlanStartDate == DateTime.MinValue) ? GetDate().AddDays(SystemConsts.Time.PlanDurationDays) : company.PlanEndDate;
        company.PlanType = planType;

        _context.Update(company);
        await _context.SaveChangesAsync();
    }
    #endregion
}