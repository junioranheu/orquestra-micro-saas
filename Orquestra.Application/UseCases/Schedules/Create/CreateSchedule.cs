using Mapster;
using Orquestra.Application.UseCases.Schedules.Base;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Schedules.Create;

public sealed class CreateSchedule(ScheduleBaseDependencies deps) : ScheduleBase(deps), ICreateSchedule
{
    private readonly Context _context = deps.Context;

    public async Task<ScheduleOutput> Execute(Guid userIdAuth, ScheduleInput input)
    {
        await Validate(input, userIdAuth, isCreate: true);
        Schedule schedule = await Save(input);

        var output = schedule.Adapt<ScheduleOutput>();
        output.DateEnd = output.Date.AddMinutes(output.DurationMinutes);
        output.Observations = await CheckForObservations(output);
        output.UsersOutput = await GetUsers(output.UsersIds);

        return output;
    }

    #region extras
    private async Task<Schedule> Save(ScheduleInput input)
    {
        var schedule = input.Adapt<Schedule>();

        schedule.DurationMinutes = GetDatesDiffInMinutes(start: input.Date, end: input.DateEnd);
        schedule.IsRestrictForSpecificUsers = input.UsersIds?.Length > 0;

        await _context.AddAsync(schedule);
        await _context.SaveChangesAsync();

        return schedule;
    }
    #endregion
}