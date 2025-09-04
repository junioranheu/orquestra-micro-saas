using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Companies.UpdateModule;

public sealed class UpdateModuleCompany(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IUpdateModuleCompany
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task Execute(Guid userIdAuth, Guid companyId, ModuleEnum[] modules)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: true);

        var result = await _context.Companies.
                     AsNoTracking().
                     Where(x => x.CompanyId == companyId).
                     FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warn_NeedToVerifyCompany);

        if (!result.Status)
        {
            throw new InvalidOperationException($"A empresa {result.Name} está desativada.");
        }

        result.Modules = modules;

        _context.Update(result);
        await _context.SaveChangesAsync();
    }
}