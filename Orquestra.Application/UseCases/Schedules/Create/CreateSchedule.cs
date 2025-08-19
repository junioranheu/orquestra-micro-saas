using Mapster;
using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Schedules.Base;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Schedules.Create;

public sealed class CreateSchedule(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser, IGetClient getClient, IGetCompany getCompany) :
    ScheduleBase(context,checkIfUserIsLinkedCompanyUser, getClient, getCompany), ICreateSchedule
{
    private readonly Context _context = context;

    public async Task<ScheduleOutput> Execute(Guid userId, ScheduleInput input)
    {
        await Validate(input, userId);
        Schedule schedule = await Save(input);

        var output = schedule.Adapt<ScheduleOutput>();

        return output;
    }

    #region extras
    private async Task<Schedule> Save(ScheduleInput input)
    {
        var schedule = input.Adapt<Schedule>();

        await _context.AddAsync(schedule);
        await _context.SaveChangesAsync();

        return schedule;
    }
    #endregion
}