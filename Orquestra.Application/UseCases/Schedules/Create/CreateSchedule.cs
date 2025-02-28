using AutoMapper;
using Orquestra.Application.UseCases.Schedules.Base;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Schedules.Create;

public sealed class CreateSchedule(Context context, IMapper map) : ScheduleBase(context), ICreateSchedule
{
    private readonly Context _context = context;
    private readonly IMapper _map = map;

    public async Task<ScheduleOutput> Execute(Guid userId, ScheduleInput input)
    {
        await Validate(input, userId, isCreate: true);
        Schedule schedule = await Save(input);

        ScheduleOutput? output = _map.Map<ScheduleOutput>(schedule);

        return output;
    }

    #region extras
    private async Task<Schedule> Save(ScheduleInput input)
    {
        Schedule schedule = _map.Map<Schedule>(input);

        await _context.AddAsync(schedule);
        await _context.SaveChangesAsync();

        return schedule;
    }
    #endregion
}