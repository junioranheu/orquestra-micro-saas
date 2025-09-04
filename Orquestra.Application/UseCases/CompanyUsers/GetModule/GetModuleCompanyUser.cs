using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.CompanyUsers.GetModule;

public sealed class GetModuleCompanyUser(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IGetModuleCompanyUser
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task<(ModuleEnum[] modules, List<string> modulesStr)> Execute(Guid userIdAuth, Guid companyId)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: false);

        var result = await _context.CompanyUsers.
                     AsNoTracking().
                     Where(x => x.CompanyId == companyId && x.UserId == userIdAuth).
                     FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warn_NotFound_User);

        if (!result.Status)
        {
            throw new InvalidOperationException(SystemConsts.Warn_NeedToVerify_User);
        }

        ModuleEnum[] modules = result.Modules ?? [];
        List<string> modulesStr = [];

        foreach (var module in modules)
        {
            modulesStr.Add(GetEnumDesc(module));
        }

        return (modules, modulesStr);
    }
}