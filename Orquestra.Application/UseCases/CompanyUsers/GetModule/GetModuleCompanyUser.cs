using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.CompanyUsers.GetModule;

public sealed class GetModuleCompanyUser(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IGetModuleCompanyUser
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    private static readonly Func<Context, Guid, Guid, Task<CompanyUser?>> _compiledGetUser =
        EF.CompileAsyncQuery((Context ctx, Guid companyId, Guid userIdAuth) =>
            ctx.CompanyUsers.
                AsNoTracking().
                Include(x => x.User).
                Include(x => x.Company).
                Where(x => x.CompanyId == companyId && x.UserId == userIdAuth).
                OrderByDescending(x => x.CreatedDate).
                FirstOrDefault());

    public async Task<(ModuleEnum[] modules, List<string> modulesStr)> Execute(Guid userIdAuth, Guid companyId)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: false);

        CompanyUser result = await _compiledGetUser(_context, companyId, userIdAuth) ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundUser);

        if (!result.Status || result.User is null || (result.User is not null && !result.User.Status))
        {
            throw new InvalidOperationException(SystemConsts.Warnings.NeedToVerifyUser);
        }

        ModuleEnum[] modules = result.UserModules ?? [];
        List<string> modulesStr = [];

        foreach (var module in modules)
        {
            modulesStr.Add(GetEnumDesc(module));
        }

        return (modules, modulesStr);
    }
}