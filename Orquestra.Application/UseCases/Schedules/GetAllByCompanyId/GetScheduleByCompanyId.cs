using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Schedules.GetAllByCompanyId;

public sealed class GetScheduleByCompanyId(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IGetScheduleByCompanyId
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task<List<ScheduleOutput>?> Execute(Guid userId, Guid companyId)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId, needAdmin: false);

        var result = await _context.Schedules.
                     Include(x => x.Clients).
                     Include(x => x.Companies).
                     AsNoTracking().
                     Where(x => x.CompanyId == companyId && x.Status == true).
                     ToListAsync();

        var output = result.Adapt<List<ScheduleOutput>>();

        // TO DO: OBTER OBSERVAÇÕES;

        return output;
    }
}