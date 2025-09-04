using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Companies.UpdateModule;

public sealed class UpdateModuleCompany(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IUpdateModuleCompany
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task Execute(Guid userIdAuth, CompanyUpdateModuleInput input)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: input.CompanyId, userId: userIdAuth, needCompanyAdmin: true);

        // TO DO: Criar lógica para cobrar $;

        var result = await _context.Companies.
                     AsNoTracking().
                     Where(x => x.CompanyId == input.CompanyId).
                     FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warn_NotFound_Company);

        if (!result.Status)
        {
            throw new InvalidOperationException(SystemConsts.Warn_NeedToVerify_Company);
        }

        // Atualizar os módulos dos funcionário dessa empresa, removendo os módulos não válidos;
        await UpdateAllModulesFromUsersOfThisCompany(oldModules: result.Modules, newModules: input.Modules, companyId: input.CompanyId);

        // Atualizar o módulo da empresa, em si;
        result.Modules = input.Modules;
        _context.Update(result);
        await _context.SaveChangesAsync();
    }

    #region extras
    private async Task UpdateAllModulesFromUsersOfThisCompany(ModuleEnum[]? oldModules, ModuleEnum[]? newModules, Guid companyId)
    {
        // Obter todos os módulos que NÃO farão mais parte da lista de módulos da empresa;
        ModuleEnum[]? invalidModules = (newModules is null || newModules.Length == 0) ? oldModules : [.. newModules?.Except(oldModules ?? []) ?? []];

        if (invalidModules is null || invalidModules.Length == 0)
        {
            return;
        }

        var result = await _context.CompanyUsers.
                     AsNoTracking().
                     Where(x =>
                        x.CompanyId == companyId &&
                        !x.Modules!.Any(y => invalidModules.Contains(y))
                     ).ToListAsync();

        if (result is null || result.Count == 0)
        {
            return;
        }

        foreach (var item in result)
        {
            ModuleEnum[] validModules = [.. (invalidModules ?? []).Except(item.Modules ?? [])];
            item.Modules = validModules;
        }

        _context.UpdateRange(result);
        await _context.SaveChangesAsync();
    }
    #endregion
}