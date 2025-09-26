using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Schedules.Base;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Schedules.Update;

public sealed class UpdateSchedule(ScheduleBaseDependencies deps) : ScheduleBase(deps), IUpdateSchedule
{
    private readonly Context _context = deps.Context;

    public async Task<ScheduleOutput> Execute(Guid userIdAuth, ScheduleInput input)
    {
        await Validate(input, userIdAuth, isCreate: false);
        Schedule schedule = await Update(input);

        var output = schedule.Adapt<ScheduleOutput>();
        output.Observations = await CheckForObservations(output);
        output.UsersOutput = await GetUsers(output.UsersIds);

        return output;
    }

    #region extras
    private async Task<Schedule> Update(ScheduleInput input)
    {
        Schedule? schedule = await _context.Schedules.
                             // AsNoTracking(). // Propositalmente sem AsNoTracking;
                             Where(x => x.ScheduleId == input.ScheduleId).
                             FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warn_NotFound_Schedule);

        schedule.DateStart = input.DateStart;
        schedule.DateEnd = input.DateEnd;
        schedule.PaymentType = input.PaymentType;
        schedule.ScheduleStatus = input.ScheduleStatus;
        schedule.ClientId = input.ClientId;
        schedule.UsersIds = input.UsersIds;
        schedule.CustomTitle = input.CustomTitle;
        schedule.CustomUrl = input.CustomUrl;
        schedule.Observation = input.Observation;
        schedule.AmountReceived = input.AmountReceived;
        schedule.IsRestrictForSpecificUsers = input.UsersIds?.Length > 0;

        _context.Update(schedule);
        await _context.SaveChangesAsync();

        return schedule;
    }
    #endregion
}