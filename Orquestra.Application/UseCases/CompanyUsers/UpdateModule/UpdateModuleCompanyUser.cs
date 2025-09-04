using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.GetModule;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;

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

    public async Task Execute(Guid userIdAuth, Guid companyId, ModuleEnum[] modules)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: true);

        CompanyModulesOutput companyModulesOutput = await _getModuleCompany.Execute(userIdAuth, companyId) ?? throw new InvalidOperationException("Nenhum módulo está vinculado à empresa, portanto você não pode prosseguir com esta ação.");
        
        var result = await _context.CompanyUsers.
                     AsNoTracking().
                     Where(x => x.CompanyId == companyId && x.UserId == userIdAuth).
                     FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warn_NeedToVerify_User);

        if (!result.Status)
        {
            throw new InvalidOperationException(SystemConsts.Warn_NeedToVerify_User);
        }

        CheckIfAnyModuleIsNotValidAccordingToCompaniesModules(companyModulesOutput, newModules: modules);

        // Atualizar/sobrescrever;
        result.Modules = modules;

        _context.Update(result);
        await _context.SaveChangesAsync();
    }

    #region extras
    private static void CheckIfAnyModuleIsNotValidAccordingToCompaniesModules(CompanyModulesOutput companyModulesOutput, ModuleEnum[] newModules)
    {
        ModuleEnum[] companiesModules = companyModulesOutput.Modules ?? [];

        // Obter todos os módulos que NÃO estão na lista da empresa;
        ModuleEnum[] invalidModules = [.. newModules.Except(companiesModules)];

        if (invalidModules.Length != 0)
        {
            throw new Exception($"Os módulos a seguir não são válidos para a empresa: {string.Join(", ", invalidModules)}");
        }
    }
    #endregion
}