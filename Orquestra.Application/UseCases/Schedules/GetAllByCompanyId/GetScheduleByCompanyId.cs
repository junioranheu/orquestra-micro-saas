using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Schedules.Base;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Schedules.GetAllByCompanyId;

public sealed class GetScheduleByCompanyId(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser, IGetClient getClient, IGetCompany getCompany) :
    ScheduleBase(context, checkIfUserIsLinkedCompanyUser, getClient, getCompany), IGetScheduleByCompanyId
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

        foreach (var item in output)
        {
            item.Observations = await CheckForObservations(item);
        }

        return output;
    }
}