using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Schedules.Base;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Schedules.GetAllByCompanyId;

public sealed class GetScheduleByCompanyId(ScheduleBaseDependencies deps) : ScheduleBase(deps), IGetScheduleByCompanyId
{
    private readonly Context _context = deps.Context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = deps.CheckIfUserIsLinkedCompanyUser;

    public async Task<List<ScheduleOutput>?> Execute(Guid userIdAuth, Guid companyId, int? year, int? month, bool? getOnlyNearbyDates = false)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: false);

        var query = _context.Schedules.
                    Include(x => x.Client).
                    Include(x => x.Company).
                    AsNoTracking().
                    Where(x => x.CompanyId == companyId && x.Status == true && x.Client!.Status == true);

        if (getOnlyNearbyDates.GetValueOrDefault())
        {
            // Ontem, hoje e amanhã, apenas;
            DateTime today = GetDate();
            DateTime yesterday = today.Date.AddDays(-1);
            DateTime tomorrow = today.Date.AddDays(2).AddTicks(-1);

            query = query.Where(x => x.DateStart >= yesterday && x.DateStart <= tomorrow);
        }
        else if ((year.HasValue && year.Value > 0) && (month.HasValue && month.Value > 0)) // Se foi passado year e month, deve pegar o intervalo de: mês anterior, mês alvo e mês posterior;
        {
            // Ano alvo;
            DateTime targetDate = new(year.Value, month.Value, 1);

            // Mês anterior;
            DateTime prev = targetDate.AddMonths(-1);

            // Próximo mês;
            DateTime next = targetDate.AddMonths(1);

            // Pega o menor e maior limite de data;
            DateTime start = prev;
            DateTime end = next.AddMonths(1).AddTicks(-1); // Fim do próximo mês;

            query = query.Where(x => x.DateStart >= start && x.DateStart <= end);
        }
        else if ((year.HasValue && year.Value > 0) && (month == 0 || month is null)) // Se foi passado apenas year, pegue apenas o ano alvo;
        {
            query = query.Where(x => x.DateStart.Year == year);
        }

        List<Schedule> result = await query.OrderBy(x => x.DateStart).ToListAsync();

        if (result is null || result.Count == 0)
        {
            return [];
        }

        var output = result.Adapt<List<ScheduleOutput>>();

        foreach (var item in output)
        {
            item.Observations = await CheckForObservations(item);
            item.UsersOutput = await GetUsers(item.UsersIds);
        }

        return output;
    }
}