using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.CompanyUsers.Get;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Domain.Enums;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Schedules.Base;

public partial class ScheduleBase(IGetCompanyUser getCompanyUser, IGetClient getClient, IGetCompany getCompany)
{
    private readonly IGetCompanyUser _getCompanyUser = getCompanyUser;
    private readonly IGetClient _getClient = getClient;
    private readonly IGetCompany _getCompany = getCompany;

    public async Task Validate(ScheduleInput input, Guid userId)
    {
        await _getCompanyUser.CheckIfUserIsFromCompany(companyId: input.CompanyId, userId, isAdmin: true);

        if (input.ScheduleStatus != ScheduleStatusEnum.Scheduled)
        {
            throw new Exception("O status de uma consulta recém criada deve ser Marcado.");
        }

        if (input.Date <= GetDate())
        {
            throw new Exception("Você não pode agendar uma consulta com a data anterior a de hoje.");
        }

        _ = await _getClient.Execute(input.ClientId) ?? throw new Exception("Esse cliente não existe.");

        _ = await _getCompany.Execute(input.CompanyId) ?? throw new Exception("Essa empresa não existe.");
    }
}