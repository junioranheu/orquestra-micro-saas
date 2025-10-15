using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.CompanyUsers.Delete;

public sealed class DeleteCompanyUser(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IDeleteCompanyUser
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task Execute(Guid userIdAuth, Guid companyId, Guid userId)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: companyId, userId: userIdAuth, needCompanyAdmin: false);

        CompanyUser userAuth = await GetUser(companyId, userId: userIdAuth);

        // O usuário autenticado é um administrador da empresa OU o usuário autenticado é, de fato, o usuário alvo em questão;
        bool hasPermission = (userAuth.CompanyUserRole == CompanyUserRoleEnum.Administrator) || (userIdAuth == userId);

        if (!hasPermission)
        {
            throw new UnauthorizedAccessException("Você não tem permissão para remover este usuário da empresa.");
        }

        bool isOwnerRemovingHimself = (userAuth.InviterUserId is null || userAuth.InviterUserId == Guid.Empty) && (userAuth.CompanyUserRole == CompanyUserRoleEnum.Administrator) && (userIdAuth == userId);

        if (isOwnerRemovingHimself)
        {
            throw new UnauthorizedAccessException($"Você é o proprietário dessa empresa, portanto não é possível remover si próprio dessa forma. Caso haja necessidade, contate o suporte: {SystemConsts.App.Email}.");
        }

        CompanyUser userAim = await GetUser(companyId, userId: userId);

        _context.Remove(userAim);
        await _context.SaveChangesAsync();
    }

    #region extras
    private async Task<CompanyUser> GetUser(Guid companyId, Guid userId)
    {
        CompanyUser user = await _context.CompanyUsers.
                           AsNoTracking().
                           Where(x =>
                              x.CompanyId == companyId &&
                              x.UserId == userId &&
                              x.Status == true
                           ).FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundUser);

        return user;
    }
    #endregion
}