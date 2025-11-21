using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using System.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.CompanyUsers.GetCurrentMain;

public sealed class GetCurrentMainCompanyUser(Context context) : IGetCurrentMainCompanyUser
{
    private readonly Context _context = context;

    #region compiled_queries
    private static readonly Func<Context, Guid, Task<CompanyUser?>> _compiledGet =
        EF.CompileAsyncQuery((Context context, Guid id) =>
            context.CompanyUsers.
            AsNoTracking().
            Include(x => x.Company).
            FirstOrDefault(x => x.UserId == id && x.IsCurrentMainCompanyUser == true && x.Status == true));

    private static readonly Func<Context, Guid, Task<CompanyUser?>> _compiledGetSimple =
        EF.CompileAsyncQuery((Context context, Guid id) =>
            context.CompanyUsers.
            AsNoTracking().
            FirstOrDefault(x => x.UserId == id && x.IsCurrentMainCompanyUser == true && x.Status == true));
    #endregion

    public async Task<(CompanyOutput? currentMainCompany, bool isUserAdm)> GetCurrentMainCompany(Guid userId)
    {
        CompanyUser? output = await _compiledGet(_context, userId);

        if (output is null || output?.Company is null)
        {
            return (null, false);
        }

        CompanyUserRoleEnum companyUserRole = output.CompanyUserRole;

        // Obter o CompanyOutput ignorando a prop CompanyUsers;
        TypeAdapterConfig config = new();
        config.NewConfig<Company, CompanyOutput>().Ignore(x => x.CompanyUsers!);
        CompanyOutput outputAdapt = output.Company.Adapt<CompanyOutput>(config);

        // Normalizar a prop Modules;
        (ModuleEnum[] modules, List<string> modulesStr) = NormalizeModules(companyUser: output, companyUserRole);
        outputAdapt.UserModules = modules;
        outputAdapt.UserModulesStr = modulesStr;

        // Normalizar outras props;
        bool isUserAdm = companyUserRole == CompanyUserRoleEnum.Administrator;
        outputAdapt.IsAdm = isUserAdm;
        outputAdapt.IsOwner = output.InviterUserId is null || output.InviterUserId == Guid.Empty;
        outputAdapt.CompanySituationStr = GetEnumDesc(outputAdapt.CompanySituation);

        return (outputAdapt, isUserAdm);
    }

    /// <summary>
    ///  Simples, sem inner joins e com menos normalizações, servindo apenas para obter os módulos da empresa atual de um usuário;
    /// </summary>
    public async Task<(ModuleEnum[] modules, List<string> modulesStr)> GetCurrentModules(Guid userId)
    {
        CompanyUser? output = await _compiledGetSimple(_context, userId);

        if (output is null)
        {
            return ([], []);
        }

        (ModuleEnum[] modules, List<string> modulesStr) = NormalizeModules(companyUser: output, companyUserRole: output.CompanyUserRole);

        return (modules, modulesStr);
    }

    #region extras
    private static (ModuleEnum[] modules, List<string> modulesStr) NormalizeModules(CompanyUser companyUser, CompanyUserRoleEnum companyUserRole)
    {
        if (companyUser is null)
        {
            return ([], []);
        }

        ModuleEnum[] modules = [];
        List<string> modulesStr = [];

        if (companyUserRole == CompanyUserRoleEnum.Administrator)
        {
            var moduleEnum = GetEnumListWithDescriptions<ModuleEnum>();

            modules = [.. moduleEnum.Select(x => x.Value)];
            modulesStr = [.. moduleEnum.Select(x => x.Description)];
        }
        else
        {
            modules = companyUser?.UserModules ?? [];

            foreach (var module in modules)
            {
                modulesStr.Add(GetEnumDesc(module));
            }
        }

        return (modules, modulesStr);
    }
    #endregion
}