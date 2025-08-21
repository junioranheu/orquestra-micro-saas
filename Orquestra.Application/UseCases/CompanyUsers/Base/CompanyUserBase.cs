using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.CompanyUsers.Base;

public partial class CompanyUserBase(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser)
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task Validate(CompanyUserInput input, Guid userIdAuth, bool isCreate)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: input.CompanyId, userId: userIdAuth, needCompanyAdmin: true);

        if (input.CompanyUserRole == CompanyUserRoleEnum.Owner)
        {
            bool hasOtherOwner = await _context.CompanyUsers.AsNoTracking().AnyAsync(x => x.CompanyId == input.CompanyId && x.CompanyUserRole == CompanyUserRoleEnum.Owner && x.Status == true);

            if (hasOtherOwner)
            {
                throw new Exception($"Essa empresa atualmente está em nome de outro {GetEnumDesc(CompanyUserRoleEnum.Owner)}. O proprietário deve, diretamente de sua conta, transferir a posse da empresa.");
            }
        }

        if (!isCreate)
        {
            Company? company = await _context.Companies.
                               AsNoTracking().
                               Where(x => x.CompanyId == input.CompanyId && x.Status == true).
                               FirstOrDefaultAsync() ?? throw new Exception("A empresa não foi contrada na base de dados.");

            if (!company.IsAccountVerified)
            {
                throw new Exception(SystemConsts.Warn_NeedToVerifyCompany);
            }
        }

        CompanyUser? companyUser = await _context.CompanyUsers.
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
}