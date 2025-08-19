using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Schedules.Base;

public partial class ScheduleBase(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser, IGetClient getClient, IGetCompany getCompany)
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;
    private readonly IGetClient _getClient = getClient;
    private readonly IGetCompany _getCompany = getCompany;

    public async Task Validate(ScheduleInput input, Guid userId)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: input.CompanyId, userId, needAdmin: true);

        if (input.ScheduleStatus != ScheduleStatusEnum.Scheduled)
        {
            throw new Exception($"O status de uma consulta recém criada deve ser {GetStatusDesc(input.ScheduleStatus)}.");
        }

        if (input.Date <= GetDate())
        {
            throw new Exception("Você não pode agendar uma consulta com a data anterior a de hoje.");
        }

        _ = await _getClient.Execute(input.ClientId) ?? throw new Exception("Esse cliente não existe.");

        _ = await _getCompany.Execute(input.CompanyId) ?? throw new Exception("Essa empresa não existe.");
    }

    public async Task<List<string>> CheckForObservations(ScheduleOutput? schedule)
    {
        List<string> observations = [];

        if (schedule is null)
        {
            return observations;
        }

        if (schedule.Date == default || schedule.Date == DateTime.MinValue)
        {
            observations.Add("Agendamento sem data definida.");
        }

        if (schedule.ScheduleStatus == ScheduleStatusEnum.Scheduled && schedule.Date < GetDate())
        {
            observations.Add($"Agendamento consta como {GetStatusDesc(schedule.ScheduleStatus)}, mas já passou da data prevista.");
        }

        if (schedule.ScheduleStatus == ScheduleStatusEnum.Canceled && schedule.Date < GetDate())
        {
            observations.Add($"Agendamento consta como {GetStatusDesc(schedule.ScheduleStatus)} após a data prevista.");
        }

        if (schedule.ScheduleStatus == ScheduleStatusEnum.Completed && schedule.Date > GetDate())
        {
            observations.Add($"Agendamento consta {GetStatusDesc(schedule.ScheduleStatus)}, mas a data ainda não ocorreu.");
        }

        var conflicts = await _context.Schedules.AsNoTracking().
                        Where(x =>
                           x.CompanyId == schedule.CompanyId &&
                           x.Date.Date == schedule.Date.Date && // Mesma data;
                           x.Date.Hour == schedule.Date.Hour && // Mesma hora;
                           x.Date.Minute == schedule.Date.Minute && // Mesmo minuto;
                           x.ScheduleStatus == ScheduleStatusEnum.Scheduled &&
                           x.ScheduleId != schedule.ScheduleId // Ignora o próprio registro;
                        ).ToListAsync();

        if (conflicts.Count != 0)
        {
            string msg = conflicts.Count == 1 ? $"Existe outro agendamento ativo na mesma data e hora." : $"Existem {conflicts.Count} outros agendamentos ativos na mesma data e hora.";
            observations.Add(msg);
        }

        return observations;
    }

    private static string GetStatusDesc(ScheduleStatusEnum status)
    {
        return GetEnumDesc(status).ToLowerInvariant();
    }
}