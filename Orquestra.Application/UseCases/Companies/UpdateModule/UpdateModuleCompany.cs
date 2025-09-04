using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Companies.UpdateModule;

public sealed class UpdateModuleCompany(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IUpdateModuleCompany
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task Execute(Guid userIdAuth, CompanyUpdateModuleInput input)
    {
        // TO DO: Criar lógica para cobrar $;

        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: input.CompanyId, userId: userIdAuth, needCompanyAdmin: true);

        var result = await _context.Companies.
                     AsNoTracking().
                     Where(x => x.CompanyId == input.CompanyId).
                     FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warn_NotFound_Company);

        if (!result.Status)
        {
            throw new InvalidOperationException(SystemConsts.Warn_NeedToVerify_Company);
        }

        result.Modules = input.Modules;

        _context.Update(result);
        await _context.SaveChangesAsync();
    }
}