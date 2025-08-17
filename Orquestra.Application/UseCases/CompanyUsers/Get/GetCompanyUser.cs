using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.CompanyUsers.Get;

public sealed class GetCompanyUser(Context context, IMapper map) : IGetCompanyUser
{
    private readonly Context _context = context;
    private readonly IMapper _map = map;

    public async Task<List<CompanyUserOutput>?> Execute(Guid companyId, Guid? userId = null)
    {
        var result = await _context.CompanyUsers.
                     Include(x => x.Users).
                     AsNoTracking().
                     Where(x =>
                        (companyId == Guid.Empty || x.CompanyId == companyId) &&
                        ((userId == Guid.Empty || userId == null) || x.UserId == userId)
                     ).
                     ToListAsync();

        var output = _map.Map<List<CompanyUserOutput>>(result);

        return output;
    }

    public async Task<bool> CheckIfUserIsFromCompany(Guid? companyId, Guid? userId, bool isAdmin, bool throwError = true)
    {
        if ((companyId is null || companyId == Guid.Empty) || (userId is null || userId == Guid.Empty))
        {
            throw new Exception($"Os parâmetros {nameof(companyId)} e {nameof(userId)} devem ser preenchidos corretamente ({nameof(CheckIfUserIsFromCompany)}).");
        }

        List<CompanyUserOutput>? result = await Execute(companyId.GetValueOrDefault(), userId.GetValueOrDefault());

        if (result?.Count == 0)
        {
            if (throwError)
            {
                ThrowError(isAdmin);
            }

            return false;
        }

        CompanyUserOutput? companyUser = result?.FirstOrDefault();

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
            string message = isAdmin ? "Apenas o administrador da empresa pode executar esta ação." : "Apenas usuários vinculados à empresa podem executar esta ação.";
            throw new Exception(message);
        }
    }
}