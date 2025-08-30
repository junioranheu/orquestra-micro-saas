using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Schedules.Base;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Schedules.Update;

public sealed class UpdateSchedule(ScheduleBaseDependencies deps) : ScheduleBase(deps), IUpdateSchedule
{
    private readonly Context _context = deps.Context;

    public async Task<ScheduleOutput> Execute(Guid userIdAuth, ScheduleInput input)
    {
        await Validate(input, userIdAuth, isCreate: false);
        Schedule schedule = await Update(input);

        var output = schedule.Adapt<ScheduleOutput>();

        return output;
    }

    #region extras
    private async Task<Schedule> Update(ScheduleInput input)
    {
        Schedule? schedule = await _context.Schedules.AsNoTracking().Where(x => x.ScheduleId == input.ScheduleId).FirstOrDefaultAsync() ?? throw new KeyNotFoundException("Agendamento não encontrado.");

        schedule.Date = input.Date;
        schedule.PaymentType = input.PaymentType;
        schedule.ScheduleStatus = input.ScheduleStatus;
        schedule.ClientId = input.ClientId;

        _context.Update(schedule);
        await _context.SaveChangesAsync();

        return schedule;
    }
    #endregion
}