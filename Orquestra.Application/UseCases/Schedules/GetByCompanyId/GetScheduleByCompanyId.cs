using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Schedules.GetByCompanyId;

public sealed class GetScheduleByCompanyId(Context context) : IGetScheduleByCompanyId
{
    private readonly Context _context = context;

    public async Task<List<ScheduleOutput>?> Execute(Guid companyId)
    {
        var result = await _context.Schedules.
                     Include(x => x.Clients).
                     Include(x => x.Companies).
                     AsNoTracking().
                     Where(x => x.CompanyId == companyId && x.Status == true).
                     ToListAsync();

        var output = result.Adapt<List<ScheduleOutput>>();

        return output;
    }
}