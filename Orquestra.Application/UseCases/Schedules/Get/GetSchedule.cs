using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Schedules.Base;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Schedules.Get;

public sealed class GetSchedule(ScheduleBaseDependencies deps) : ScheduleBase(deps), IGetSchedule
{
    private readonly Context _context = deps.Context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = deps.CheckIfUserIsLinkedCompanyUser;

    public async Task<ScheduleOutput?> Execute(Guid userIdAuth, Guid scheduleId)
    {
        var result = await _context.Schedules.
                     Include(x => x.Client).
                     Include(x => x.Company).
                     AsNoTracking().
                     Where(x => x.ScheduleId == scheduleId && x.Status == true).
                     FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundSchedule);

        Guid companyId = result.CompanyId;
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: false);

        var output = result.Adapt<ScheduleOutput>();
        output.Observations = await CheckForObservations(output);
        output.UsersOutput = await GetUsers(output.UsersIds);

        return output;
    }
}