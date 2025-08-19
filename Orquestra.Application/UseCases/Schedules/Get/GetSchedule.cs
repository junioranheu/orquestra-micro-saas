using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Schedules.Get;

public sealed class GetSchedule(Context context) : IGetSchedule
{
    private readonly Context _context = context;

    public async Task<ScheduleOutput?> Execute(Guid scheduleId)
    {
        var result = await _context.Schedules.
                     Include(x => x.Clients).
                     Include(x => x.Companies).
                     AsNoTracking().
                     Where(x =>
                        x.Status == true &&
                        x.ScheduleId == scheduleId
                     ).
                     FirstOrDefaultAsync();

        var output = result.Adapt<ScheduleOutput>();

        return output;
    }
}