using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.Get;
using Orquestra.Application.UseCases.Schedules.Base;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Schedules.Update;

public sealed class UpdateSchedule(Context context, IMapper map, IGetCompanyUser getCompanyUser) : ScheduleBase(getCompanyUser), IUpdateSchedule
{
    private readonly Context _context = context;
    private readonly IMapper _map = map;

    public async Task<ScheduleOutput> Execute(Guid userId, ScheduleInput input)
    {
        await Validate(input, userId, isCreate: false);
        Schedule schedule = await Update(userId, input);

        ScheduleOutput? output = _map.Map<ScheduleOutput>(schedule);

        return output;
    }

    #region extras
    private async Task<Schedule> Update(Guid userId, ScheduleInput input)
    {
        Schedule? schedule = await _context.Schedules.AsNoTracking().Where(x => x.ScheduleId == userId).FirstOrDefaultAsync() ?? throw new Exception("Agendamento não encontrado");

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