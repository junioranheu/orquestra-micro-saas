using Microsoft.EntityFrameworkCore;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.CompanyUsers.Get;

public sealed class GetCompanyUser(Context context) : IGetCompanyUser
{
    private readonly Context _context = context;

    public async Task<List<CompanyUser>?> Execute(Guid companyId, Guid userId)
    {
        var result = await _context.CompanyUsers.
                     Include(x => x.Users).
                     AsNoTracking().
                     Where(x =>
                        (companyId == Guid.Empty || x.CompanyId == companyId) &&
                        (userId == Guid.Empty || x.UserId == userId)
                     ).
                     ToListAsync();

        return result;
    }

    public async Task<bool> CheckIfUserIsFromCompany(Guid? companyId, Guid? userId, bool isAdmin, bool throwError = true)
    {
        if ((companyId is null || companyId == Guid.Empty) || (userId is null || userId == Guid.Empty))
        {
            throw new Exception($"Os parâmetros {nameof(companyId)} e {nameof(userId)} devem ser preenchidos corretamente ({nameof(CheckIfUserIsFromCompany)})");
        }

        List<CompanyUser>? result = await Execute(companyId.GetValueOrDefault(), userId.GetValueOrDefault());

        if (result?.Count == 0)
        {
            if (throwError)
            {
                ThrowError(isAdmin);
            }
        
            return false;
        }

        CompanyUser? companyUser = result?.FirstOrDefault();

        if (isAdmin)
        {
            bool checkIfIsAdmin = companyUser?.CompanyUserRole == CompanyUserRoleEnum.Administrator;

            if (!checkIfIsAdmin && throwError)
            {
                ThrowError(isAdmin);
            }

            return checkIfIsAdmin;
        }

        return true;

        static void ThrowError(bool isAdmin)
        {
            string message = isAdmin ? "Apenas o administrador da empresa pode executar esta ação" : "Apenas usuários vinculados à empresa podem executar esta ação";
            throw new Exception(message);
        }
    }
}