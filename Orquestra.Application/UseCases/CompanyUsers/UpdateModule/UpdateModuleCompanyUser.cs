using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.GetModule;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.CompanyUsers.UpdateModule;

public sealed class UpdateModuleCompanyUser(
        Context context,
        ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser,
        IGetModuleCompany getModuleCompany
    ) : IUpdateModuleCompanyUser
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;
    private readonly IGetModuleCompany _getModuleCompany = getModuleCompany;

    public async Task Execute(Guid userIdAuth, CompanyUserUpdateModuleInput input)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: input.CompanyId, userId: userIdAuth, needCompanyAdmin: true);
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: input.CompanyId, userId: input.UserId, needCompanyAdmin: false);

        CompanyModulesOutput companyModulesOutput = await _getModuleCompany.Execute(userId: input.UserId, companyId: input.CompanyId) ?? throw new InvalidOperationException("Nenhum módulo está vinculado à empresa, portanto você não pode prosseguir com esta ação.");

        var result = await _context.CompanyUsers.
                     // AsNoTracking(). // Propositalmente sem AsNoTracking;
                     Where(x => x.CompanyId == input.CompanyId && x.UserId == input.UserId).
                     FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warn_NotFound_User);

        if (!result.Status)
        {
            throw new InvalidOperationException(SystemConsts.Warn_NeedToVerify_User);
        }

        CheckIfAnyModuleIsNotValidAccordingToCompaniesModules(companyModulesOutput, newModules: input.Modules);

        // Atualizar/sobrescrever;
        result.Modules = [.. input.Modules?.Distinct() ?? []];

        _context.Update(result);
        await _context.SaveChangesAsync();
    }

    #region extras
    private static void CheckIfAnyModuleIsNotValidAccordingToCompaniesModules(CompanyModulesOutput companyModulesOutput, ModuleEnum[]? newModules)
    {
        ModuleEnum[] companiesModules = companyModulesOutput.Modules ?? [];

        if (newModules is null || newModules.Length == 0)
        {
            return;
        }

        // Obter todos os módulos que NÃO estão na lista da empresa;
        ModuleEnum[] invalidModules = [.. newModules.Except(companiesModules)];

        if (invalidModules.Length != 0)
        {
            List<string> invalidModulesStr = [];

            foreach (var module in invalidModules)
            {
                invalidModulesStr.Add(GetEnumDesc(module));
            }

            string msg = invalidModules.Length == 1 ? 
                $"O módulo a seguir não é válido para a empresa: {string.Join(", ", invalidModulesStr)}" : 
                $"Os módulos a seguir não são válidos para a empresa: {string.Join(", ", invalidModulesStr)}";

            throw new InvalidOperationException(msg);
        }
    }
    #endregion
}