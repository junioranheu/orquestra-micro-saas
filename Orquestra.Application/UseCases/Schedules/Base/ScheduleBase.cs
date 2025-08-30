using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Schedules.Base;

public record ScheduleBaseDependencies(
    Context Context,
    ICheckIfUserIsLinkedCompanyUser CheckIfUserIsLinkedCompanyUser,
    IGetClient GetClient,
    IGetCompany GetCompany
);

public partial class ScheduleBase(ScheduleBaseDependencies deps)
{
    private readonly Context _context = deps.Context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = deps.CheckIfUserIsLinkedCompanyUser;
    private readonly IGetClient _getClient = deps.GetClient;
    private readonly IGetCompany _getCompany = deps.GetCompany;

    public async Task Validate(ScheduleInput input, Guid userIdAuth, bool isCreate)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: input.CompanyId, userId: userIdAuth, needCompanyAdmin: false);

        if (isCreate)
        {
            if (input.ScheduleStatus != ScheduleStatusEnum.Scheduled)
            {
                throw new ArgumentException($"O status de uma consulta recém criada deve ser {GetStatusDesc(ScheduleStatusEnum.Scheduled)}.");
            }
        }

        if (input.Date <= GetDate())
        {
            throw new ArgumentException("Você não pode agendar uma consulta com a data anterior a de hoje.");
        }

        _ = await _getClient.Execute(userIdAuth: userIdAuth, clientId: input.ClientId) ?? throw new KeyNotFoundException("Esse cliente não existe.");

        _ = await _getCompany.Execute(userIdAuth: userIdAuth, companyId: input.CompanyId, throwIfStatusFalse: true) ?? throw new KeyNotFoundException("Essa empresa não existe.");

        await ValidateUsersAndRemoveNotLinkedOnes(input);
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
            observations.Add($"Agendamento consta como {GetStatusDesc(ScheduleStatusEnum.Scheduled)}, mas já passou da data prevista.");
        }

        if (schedule.ScheduleStatus == ScheduleStatusEnum.Canceled && schedule.Date < GetDate())
        {
            observations.Add($"Agendamento consta como {GetStatusDesc(ScheduleStatusEnum.Canceled)} após a data prevista.");
        }

        if (schedule.ScheduleStatus == ScheduleStatusEnum.Completed && schedule.Date > GetDate())
        {
            observations.Add($"Agendamento consta {GetStatusDesc(ScheduleStatusEnum.Completed)}, mas a data ainda não ocorreu.");
        }

        var conflicts = await _context.Schedules.
                        AsNoTracking().
                        Where(x =>
                           x.CompanyId == schedule.CompanyId &&
                           x.Date.Date == schedule.Date.Date && // Mesma data;
                           x.Date.Hour == schedule.Date.Hour && // Mesma hora;
                           x.Date.Minute == schedule.Date.Minute && // Mesmo minuto;
                           x.ScheduleStatus == ScheduleStatusEnum.Scheduled &&
                           x.ScheduleId != schedule.ScheduleId && // Ignora o próprio registro;
                           x.Status == true
                        ).ToListAsync();

        if (conflicts.Count != 0)
        {
            string msg = conflicts.Count == 1 ? $"Existe outro agendamento ativo na mesma data e hora." : $"Existem {conflicts.Count} outros agendamentos ativos na mesma data e hora.";
            observations.Add(msg);
        }

        return observations;
    }

    public async Task<UserOutput[]> GetUsers(Guid[]? users)
    {
        if (users is null || users?.Length == 0)
        {
            return [];
        }

        var result = await _context.Users.
                     AsNoTracking().
                     Where(x => users!.Contains(x.UserId) && x.Status == true).
                     ToListAsync();

        if (result?.Count == 0)
        {
            return [];
        }

        var output = result.Adapt<List<UserOutput>>();

        return [.. output];
    }

    #region extras
    private static string GetStatusDesc(ScheduleStatusEnum status)
    {
        return GetEnumDesc(status).ToLowerInvariant();
    }

    private async Task ValidateUsersAndRemoveNotLinkedOnes(ScheduleInput input)
    {
        Guid[]? usersIds = input.UsersIds;

        if (usersIds is null || usersIds.Length == 0)
        {
            return;
        }

        List<Guid> validUsers = [];

        foreach (var item in usersIds)
        {
            bool existAndIsLinkedToCompany = await _context.CompanyUsers.
                                             AsNoTracking().
                                             AnyAsync(x => x.CompanyId == input.CompanyId && x.UserId == item && x.Status == true);

            if (existAndIsLinkedToCompany)
            {
                validUsers.Add(item);
            }
        }

        if (validUsers is null || validUsers.Count == 0)
        {
            string msg = usersIds.Length == 1 ? 
                "O usuário que você referenciou para o agendamento não é válido ou não está vinculado à empresa." :
                $"Os {usersIds.Length} usuários que você referenciou para o agendamento não são válidos ou não estão vinculados à empresa.";

            throw new UnauthorizedAccessException(msg);
        }

        input.UsersIds = [.. validUsers];
    }
    #endregion
}