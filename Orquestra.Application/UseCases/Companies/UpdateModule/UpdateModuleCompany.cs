using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyInvoices.Create;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Companies.UpdateModule;

public sealed class UpdateModuleCompany(
        Context context,
        ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser,
        ICreateCompanyInvoice createCompanyInvoice,
        IGetUser getUser
    ) : IUpdateModuleCompany
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;
    private readonly ICreateCompanyInvoice _createCompanyInvoice = createCompanyInvoice;
    private readonly IGetUser _getUser = getUser;

    public async Task Execute(Guid userIdAuth, CompanyUpdateModuleInput input)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: input.CompanyId, userId: userIdAuth, needCompanyAdmin: true);

        var result = await _context.Companies.
                     // AsNoTracking(). // Propositalmente sem AsNoTracking;
                     Where(x => x.CompanyId == input.CompanyId).
                     FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundCompany);

        if (!result.Status)
        {
            throw new InvalidOperationException(SystemConsts.Warnings.NeedToVerifyCompany);
        }

        // #1 - Criar cobrança, obrigatoriamente antes de normalizar o input.Modules;
        await _createCompanyInvoice.Execute(userIdAuth, companyId: input.CompanyId, modules: input.CompanyModules ?? []);

        // #2 - Se o usuário for ADM do sistema, ele pode remover os módulos;
        // Se o usuário não for ADM do sistema, ele NÃO pode remover, e aí automaticamente o input é normalizado;
        await NormalizeInputModulesIfNotSystemAdmin(userIdAuth, input, result);

        // #3 - Atualizar os dados da empresa, em si;
        await UpdateCompanyData(company: result, input);
    }

    #region extras
    private async Task NormalizeInputModulesIfNotSystemAdmin(Guid userIdAuth, CompanyUpdateModuleInput input, Company company)
    {
        UserOutput user = await _getUser.Execute(userId: userIdAuth, throwIfStatusFalse: true);

        // ADM do sistema não normaliza o input, permitindo remover módulos, se necessário;
        if (user.Role is UserRoleEnum.Administrator || user.Role is UserRoleEnum.Maintainer)
        {
            return;
        }

        // Usuário comum normaliza o input, NÃO permitindo remover módulos;
        ModuleEnum[]? newModules = input.CompanyModules;
        ModuleEnum[]? existentModules = company.CompanyModules;
        ModuleEnum[]? output = existentModules?.Concat(newModules ?? []).Distinct().ToArray();

        input.CompanyModules = output;
    }

    private async Task UpdateCompanyData(Company company, CompanyUpdateModuleInput input)
    {
        // Atualizar os módulos dos funcionário dessa empresa, removendo os módulos não válidos;
        await UpdateAllModulesFromUsersOfThisCompany(oldModules: company.CompanyModules, newModules: input.CompanyModules, companyId: input.CompanyId);

        // Atualizar dados da empresa;
        company.CompanySituation = input.CompanyModules?.Length >= 1 ? CompanySituationEnum.PendingPayment : CompanySituationEnum.RegisteredButWithoutAnyModules;
        company.PlanStartDate = (company.PlanStartDate is null || company.PlanStartDate == DateTime.MinValue) ? GetDate() : company.PlanStartDate;
        company.PlanEndDate = (company.PlanStartDate is null || company.PlanStartDate == DateTime.MinValue) ? GetDate().AddDays(SystemConsts.Time.PlanDurationDays) : company.PlanEndDate;
        company.CompanyModules = input.CompanyModules;

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
            ModuleEnum[] validModules = [.. (item.UserModules ?? []).Except(invalidModules ?? [])];
            item.UserModules = [.. validModules.Distinct()];
        }

        _context.UpdateRange(result);
        await _context.SaveChangesAsync();
    }
    #endregion
}