using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUser;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.CompanyUsers.Base;

public partial class CompanyUserBase(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser)
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task Validate(CompanyUserInput input, Guid userId, bool isCreate)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: input.CompanyId, userId, needAdmin: true);

        if (isCreate)
        {
            CompanyUser? companyUser = await _context.CompanyUsers.AsNoTracking().Where(x => x.CompanyId == input.CompanyId && x.UserId == input.UserId && x.Status == true).FirstOrDefaultAsync();

            if (companyUser is not null)
            {
                User? user = await _context.Users.AsNoTracking().Where(x => x.UserId == companyUser.UserId).FirstOrDefaultAsync();
                throw new Exception($"O usuário {user?.FullName ?? input.UserId.ToString()} já está cadastrado nessa empresa.");
            }
        }
    }
}