using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyInvoices.Create;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Companies.UpdateModule;

public sealed class UpdateModuleCompany(
        Context context,
        ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser,
        ICreateCompanyInvoice createCompanyInvoice
    ) : IUpdateModuleCompany
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;
    private readonly ICreateCompanyInvoice _createCompanyInvoice = createCompanyInvoice;

    public async Task Execute(Guid userIdAuth, CompanyUpdateModuleInput input)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: input.CompanyId, userId: userIdAuth, needCompanyAdmin: true);

        var result = await _context.Companies.
                     // AsNoTracking(). // Propositalmente sem AsNoTracking;
                     Where(x => x.CompanyId == input.CompanyId).
                     FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warn_NotFound_Company);

        if (!result.Status)
        {
            throw new InvalidOperationException(SystemConsts.Warn_NeedToVerify_Company);
        }

        // Atualizar os dados da empresa, em si;
        await UpdateCompanyData(company: result, input);

        // Criar cobrança;
        await _createCompanyInvoice.Execute(userIdAuth, companyId: input.CompanyId, modules: input.Modules ?? []);
    }

    #region extras
    private async Task UpdateCompanyData(Company company, CompanyUpdateModuleInput input)
    {
        // Atualizar os módulos dos funcionário dessa empresa, removendo os módulos não válidos;
        await UpdateAllModulesFromUsersOfThisCompany(oldModules: company.Modules, newModules: input.Modules, companyId: input.CompanyId);

        // Atualizar dados da empresa;
        company.CompanySituation = input.Modules?.Length >= 1 ? CompanySituationEnum.RegisteredButWithoutAnyModules : CompanySituationEnum.PendingPayment;
        company.PlanStartDate = (company.PlanStartDate is null || company.PlanStartDate == DateTime.MinValue) ? GetDate() : company.PlanStartDate;
        company.PlanEndDate = (company.PlanStartDate is null || company.PlanStartDate == DateTime.MinValue) ? GetDate().AddDays(30) : company.PlanEndDate;
        company.Modules = input.Modules;

        _context.Update(company);
        await _context.SaveChangesAsync();
    }

    private async Task UpdateAllModulesFromUsersOfThisCompany(ModuleEnum[]? oldModules, ModuleEnum[]? newModules, Guid companyId)
    {
        // Obter todos os módulos que NÃO farão mais parte da lista de módulos da empresa;
        ModuleEnum[]? invalidModules = (newModules is null || newModules.Length == 0) ? oldModules : [.. oldModules?.Except(newModules ?? []) ?? []];

        if (invalidModules is null || invalidModules.Length == 0)
        {
            return;
        }

        var result = await _context.CompanyUsers.
                     // AsNoTracking(). // Propositalmente sem AsNoTracking;
                     Where(x => x.CompanyId == companyId).ToListAsync();

        if (result is null || result.Count == 0)
        {
            return;
        }

        foreach (var item in result)
        {
            ModuleEnum[] validModules = [.. (item.Modules ?? []).Except(invalidModules ?? [])];
            item.Modules = [.. validModules.Distinct()];
        }

        _context.UpdateRange(result);
        await _context.SaveChangesAsync();
    }
    #endregion
}