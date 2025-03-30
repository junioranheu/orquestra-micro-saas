using Orquestra.Application.UseCases.CompanyUsers.Get;
using Orquestra.Application.UseCases.Schedules.Shared;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Schedules.Base;

public partial class ScheduleBase(IGetCompanyUser getCompanyUser)
{
    private readonly IGetCompanyUser _getCompanyUser = getCompanyUser;

    public async Task Validate(ScheduleInput input, Guid userId, bool isCreate)
    {
        await _getCompanyUser.CheckIfUserIsFromCompany(companyId: input.CompanyId, userId, isAdmin: true);

        if (input.Date <= GetDate())
        {
            throw new Exception("Você não pode agendar uma consulta com a data anterior a de hoje");
        }
    }
}