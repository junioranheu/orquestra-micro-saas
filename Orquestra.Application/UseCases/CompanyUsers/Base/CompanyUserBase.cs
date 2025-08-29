using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.CompanyUsers.Base;

public partial class CompanyUserBase(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser)
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task Validate(CompanyUserInput input, Guid userIdAuth, bool isCreate)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: input.CompanyId, userId: userIdAuth, needCompanyAdmin: true);

        var company = await _context.Companies.
                      AsNoTracking().
                      Where(x =>
                        x.CompanyId == input.CompanyId &&
                        (input.CompanyUserRole == CompanyUserRoleEnum.Administrator || x.Status == true)
                      ).FirstOrDefaultAsync() ?? throw new Exception("A empresa não foi contrada na base de dados.");

        List<CompanyUser> companyUsers = await _context.CompanyUsers.AsNoTracking().Where(x => x.CompanyId == input.CompanyId && x.Status == true).ToListAsync();
        bool isFirstAdministrator = CheckIfIsFirstAdministratorBeforeCreatingIt(companyUsers);

        if (!isFirstAdministrator)
        {
            if (!company.Status)
            {
                throw new Exception(SystemConsts.Warn_NeedToVerifyCompany);
            }
        }

        var companyUser = await _context.CompanyUsers.
                          AsNoTracking().
                          Where(x => x.CompanyId == input.CompanyId && x.UserId == input.UserId && x.Status == true).
                          FirstOrDefaultAsync();

        if (isCreate)
        {
            if (companyUser is not null)
            {
                User? user = await _context.Users.AsNoTracking().Where(x => x.UserId == companyUser.UserId).FirstOrDefaultAsync();
                throw new Exception($"O usuário {user?.FullName ?? input.UserId.ToString()} já está cadastrado nessa empresa.");
            }

            return;
        }

        if (!isCreate)
        {
            if (companyUser is null)
            {
                throw new Exception("O usuário não está cadastrado nessa empresa.");
            }

            return;
        }
    }

    #region extras
    private static bool CheckIfIsFirstAdministratorBeforeCreatingIt(List<CompanyUser> companyUsers)
    {
        if (companyUsers is null || companyUsers.Count == 0)
        {
            return true;
        }

        if (companyUsers.Count >= 1)
        {
            return false;
        }

        return true;
    }
    #endregion
}