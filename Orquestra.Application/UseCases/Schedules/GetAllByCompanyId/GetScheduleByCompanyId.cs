using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Schedules.Base;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Schedules.GetAllByCompanyId;

public sealed class GetScheduleByCompanyId(ScheduleBaseDependencies deps) : ScheduleBase(deps), IGetScheduleByCompanyId
{
    private readonly Context _context = deps.Context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = deps.CheckIfUserIsLinkedCompanyUser;

    public async Task<List<ScheduleOutput>?> Execute(Guid userIdAuth, Guid companyId)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: false);

        var result = await _context.Schedules.
                     Include(x => x.Client).
                     Include(x => x.Company).
                     AsNoTracking().
                     Where(x => x.CompanyId == companyId && x.Status == true).
                     ToListAsync();

        var output = result.Adapt<List<ScheduleOutput>>();

        foreach (var item in output)
        {
            item.Observations = await CheckForObservations(item);
            item.UsersOutput = await GetUsers(item.UsersIds);
        }

        return output;
    }
}