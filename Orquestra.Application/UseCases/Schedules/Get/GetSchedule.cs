using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Schedules.Get;

public sealed class GetSchedule(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IGetSchedule
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task<ScheduleOutput?> Execute(Guid userId, Guid scheduleId)
    {
        var result = await _context.Schedules.
                     Include(x => x.Clients).
                     Include(x => x.Companies).
                     AsNoTracking().
                     Where(x =>
                        x.Status == true &&
                        x.ScheduleId == scheduleId
                     ).
                     FirstOrDefaultAsync() ?? throw new Exception($"Não foi possível localizar este horário agendado. ({scheduleId})");

        Guid companyId = result.CompanyId;
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId, needAdmin: false);

        var output = result.Adapt<ScheduleOutput>();

        // TO DO: OBTER OBSERVAÇÕES;

        return output;
    }
}