using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using System.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.CompanyUsers.GetCurrentMain;

public sealed class GetCurrentMainCompanyUser(Context context) : IGetCurrentMainCompanyUser
{
    private readonly Context _context = context;

    public async Task<(CompanyOutput? currentMainCompany, bool isUserAdm)> Execute(Guid userId)
    {
        var output = await _context.CompanyUsers.
                     Include(x => x.Company).
                     AsNoTracking().
                     Where(x => x.UserId == userId && x.IsCurrentMainCompanyUser == true && x.Status == true).
                     FirstOrDefaultAsync();

        if (output is null)
        {
            return (null, false);
        }

        CompanyOutput outputAdapt = output.Company.Adapt<CompanyOutput>();
        outputAdapt.UserModules = output.UserModules;

        if (outputAdapt is null)
        {
            return (null, false);
        }

        if (outputAdapt is not null)
        {
            ModuleEnum[] modules = outputAdapt.CompanyModules ?? [];

            foreach (var module in modules)
            {
                outputAdapt.CompanyModulesStr.Add(GetEnumDesc(module));
            }

            ModuleEnum[] userModules = outputAdapt.UserModules ?? [];

            foreach (var module in userModules)
            {
                outputAdapt.UserModulesStr.Add(GetEnumDesc(module));
            }
        }

        bool isUserAdm = output.CompanyUserRole == CompanyUserRoleEnum.Administrator;

        return (outputAdapt, isUserAdm);
    }
}