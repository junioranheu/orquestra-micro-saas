using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Companies.GetModule;

public sealed class GetModuleCompany(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IGetModuleCompany
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task<CompanyModulesOutput> Execute(Guid userId, Guid companyId)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId, needCompanyAdmin: false);

        var result = await _context.Companies.
                     AsNoTracking().
                     Where(x => x.CompanyId == companyId).
                     FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warn_NotFound_Company);

        if (!result.Status)
        {
            throw new InvalidOperationException(SystemConsts.Warn_NeedToVerify_Company);
        }

        ModuleEnum[] modules = result.Modules ?? [];
        List<string> modulesStr = [];

        foreach (var module in modules)
        {
            modulesStr.Add(GetEnumDesc(module));
        }

        var company = result.Adapt<CompanySimpleOutput>();

        CompanyModulesOutput output = new()
        {
            Company = company,
            Modules = modules,
            ModulesStr = modulesStr
        };

        return output;
    }
}