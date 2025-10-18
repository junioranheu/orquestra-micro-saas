using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.CompanyUsers.Update;

public sealed class UpdateCompanyUser(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IUpdateCompanyUser
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task Execute(Guid userIdAuth, CompanyUserInput input)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: input.CompanyId, userId: userIdAuth, needCompanyAdmin: true);
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: input.CompanyId, userId: input.UserId, needCompanyAdmin: false);

        var result = await _context.CompanyUsers.
                     // AsNoTracking(). // Propositalmente sem AsNoTracking;
                     Where(x => x.CompanyId == input.CompanyId && x.UserId == input.UserId && x.Status == true).
                     FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundUser);

        if (result.CompanyUserRole == CompanyUserRoleEnum.Administrator && input.CompanyUserRole == CompanyUserRoleEnum.Member && (result.InviterUserId is null || result.InviterUserId == Guid.Empty))
        {
            throw new InvalidOperationException("Você é o proprietário dessa empresa, portanto não pode remover a sua permissão de administrador.");
        }

        // Atualizar/sobrescrever;
        result.UserModules = [.. input.UserModules?.Distinct() ?? []];
        result.CompanyUserRole = input.CompanyUserRole > 0 ? input.CompanyUserRole : result.CompanyUserRole;

        _context.Update(result);
        await _context.SaveChangesAsync();
    }
}