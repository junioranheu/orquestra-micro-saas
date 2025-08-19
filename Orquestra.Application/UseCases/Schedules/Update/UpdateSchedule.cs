using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Schedules.Base;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Schedules.Update;

public sealed class UpdateSchedule(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser, IGetClient getClient, IGetCompany getCompany) : 
    ScheduleBase(context, checkIfUserIsLinkedCompanyUser, getClient, getCompany), IUpdateSchedule
{
    private readonly Context _context = context;

    public async Task<ScheduleOutput> Execute(Guid userId, ScheduleInput input)
    {
        await Validate(input, userId);
        Schedule schedule = await Update(input);

        var output = schedule.Adapt<ScheduleOutput>();

        return output;
    }

    #region extras
    private async Task<Schedule> Update(ScheduleInput input)
    {
        Schedule? schedule = await _context.Schedules.AsNoTracking().Where(x => x.ScheduleId == input.ScheduleId).FirstOrDefaultAsync() ?? throw new Exception("Agendamento não encontrado.");

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