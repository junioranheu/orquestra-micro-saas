using AutoMapper;
using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.CompanyUsers.Get;
using Orquestra.Application.UseCases.Schedules.Base;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Schedules.Create;

public sealed class CreateSchedule(Context context, IMapper map, IGetCompanyUser getCompanyUser, IGetClient getClient, IGetCompany getCompany) : ScheduleBase(getCompanyUser, getClient, getCompany), ICreateSchedule
{
    private readonly Context _context = context;
    private readonly IMapper _map = map;

    public async Task<ScheduleOutput> Execute(Guid userId, ScheduleInput input)
    {
        await Validate(input, userId);
        Schedule schedule = await Save(input);

        var output = _map.Map<ScheduleOutput>(schedule);

        return output;
    }

    #region extras
    private async Task<Schedule> Save(ScheduleInput input)
    {
        var schedule = _map.Map<Schedule>(input);

        await _context.AddAsync(schedule);
        await _context.SaveChangesAsync();

        return schedule;
    }
    #endregion
}