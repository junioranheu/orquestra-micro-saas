using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Schedules.Base;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Schedules.GetAllByClientId;

public sealed class GetScheduleByClientId(ScheduleBaseDependencies deps) : ScheduleBase(deps), IGetScheduleByClientId
{
    private readonly Context _context = deps.Context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = deps.CheckIfUserIsLinkedCompanyUser;

    public async Task<List<ScheduleOutput>?> Execute(Guid userIdAuth, Guid companyId, Guid clientId)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: false);

        var result = await _context.Schedules.
                     AsNoTracking().
                     Where(x => x.CompanyId == companyId && x.ClientId == clientId && x.Status == true).
                     OrderByDescending(x => x.CreatedDate).
                     ToListAsync();

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