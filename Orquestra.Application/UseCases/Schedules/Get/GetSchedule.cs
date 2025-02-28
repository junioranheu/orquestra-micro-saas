using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Schedules.Get;

public sealed class GetSchedule(Context context, IMapper map) : IGetSchedule
{
    private readonly Context _context = context;
    private readonly IMapper _map = map;

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

        ScheduleOutput? output = _map.Map<ScheduleOutput>(result);

        return output;
    }

    public async Task<List<ScheduleOutput>?> Execute()
    {
        var result = await _context.Schedules.
                     Include(x => x.Clients).
                     Include(x => x.Companies).
                     AsNoTracking().
                     Where(x => x.Status == true).
                     ToListAsync();

        List<ScheduleOutput>? output = _map.Map<List<ScheduleOutput>>(result);

        return output;
    }
}