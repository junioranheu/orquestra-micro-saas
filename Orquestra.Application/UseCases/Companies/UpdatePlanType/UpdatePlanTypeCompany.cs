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

        // #1 - Checar a "disponibilidade" da alteração;
        await CheckAvailability(company: result, newPlanType: planType);

        // #2 - Criar cobrança, obrigatoriamente antes de normalizar o input.Modules;
        await _createCompanyInvoice.Execute(userIdAuth, companyId, planType);

        // #3 - Atualizar os dados da empresa, em si;
        await UpdateCompanyData(company: result, planType);
    }

    #region extras
    private async Task CheckAvailability(Company company, PlanTypeEnum newPlanType)
    {
        (decimal _, int _, string descriptionCurrentPlan, string[] _, int _) = PlanTypeHelper.GetValues(company.PlanType.GetValueOrDefault());
        // (decimal _, int _, string descriptionNewPlan, string[] _, int _) = PlanTypeHelper.GetValues(newPlanType);

        if (company.PlanType == newPlanType)
        {
            throw new InvalidOperationException($"Não foi possível prosseguir pois essa empresa já está com o plano {descriptionCurrentPlan.ToLowerInvariant()} vigente.");
        }

        bool hasAnyPendingInvoice = await _context.CompanyInvoices.AsNoTracking().AnyAsync(x => x.CompanyId == company.CompanyId && x.CompanyInvoiceSituation == CompanyInvoiceSituationEnum.Pending && x.PlanType != PlanTypeEnum.Free && x.Status == true);

        if (hasAnyPendingInvoice)
        {
            throw new InvalidOperationException($"Esta empresa tem pelo menos uma fatura em aberto, pendente de pagamento, portanto não foi possível prosseguir.");
        }
    }

    private async Task UpdateCompanyData(Company company, PlanTypeEnum planType)
    {
        company.CompanySituation = CompanySituationEnum.PendingPayment;
        company.PlanStartDate = GetDate();
        company.PlanEndDate = GetDate().AddDays(SystemConsts.Time.PlanDurationDays);
        company.PlanType = planType;

        _context.Update(company);
        await _context.SaveChangesAsync();
    }
    #endregion
}