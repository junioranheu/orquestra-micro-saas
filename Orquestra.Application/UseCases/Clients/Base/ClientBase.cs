using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.CompanyUsers.Get;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Clients.Base;

public partial class ClientBase(IGetCompanyUser getCompanyUser)
{
    private readonly IGetCompanyUser _getCompanyUser = getCompanyUser;

    public async Task Validate(ClientInput input, Guid userId, bool isCreate)
    {
        List<CompanyUser>? companiesFromUser = await _getCompanyUser.Execute(companyId: Guid.Empty, userId: userId);
        bool? isAdmin = companiesFromUser?.Any(x => x.Users?.UserId == userId && x.CompanyId == input.CompanyId && (x.CompanyUserRole == CompanyUserRoleEnum.Administrator || x.CompanyUserRole == CompanyUserRoleEnum.Owner));

        if (input.CompanyId == Guid.Empty || companiesFromUser?.Count == 0 || !isAdmin.GetValueOrDefault())
        {
            throw new Exception("Apenas um administrador da empresa pode alterar suas informações");
        }

        if (input.Date < GetDate())
        {
            throw new Exception("xxx");
        }
    }
}