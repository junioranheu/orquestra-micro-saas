using Orquestra.Application.UseCases.CompanyUsers.Get;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Schedules.Base;

public partial class ScheduleBase(Context context, IGetCompanyUser getCompanyUser)
{
    private readonly Context _context = context;
    private readonly IGetCompanyUser _getCompanyUser = getCompanyUser;

    public async Task Validate(ScheduleInput input, Guid userId, bool isCreate)
    {
        List<CompanyUser>? companiesFromUser = await _getCompanyUser.Execute(companyId: Guid.Empty, userId: userId);
        bool? isAdmin = companiesFromUser?.Any(x => x.Users?.UserId == userId && x.CompanyId == input.CompanyId && (x.CompanyUserRole == CompanyUserRoleEnum.Administrator || x.CompanyUserRole == CompanyUserRoleEnum.Owner));

        if (input.CompanyId == Guid.Empty || companiesFromUser?.Count == 0 || !isAdmin.GetValueOrDefault())
        {
            throw new Exception("Apenas um administrador da empresa pode alterar suas informações");
        }

        if (input.Date < GetDate())
        {
            throw new Exception("Você não pode agendar uma consulta com a data anterior a de hoje");
        }
    }
}