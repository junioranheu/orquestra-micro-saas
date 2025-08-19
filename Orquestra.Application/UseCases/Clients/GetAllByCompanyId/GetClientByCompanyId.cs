using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Clients.GetAllByCompanyId;

public sealed class GetClientByCompanyId(Context context) : IGetClientByCompanyId
{
    private readonly Context _context = context;

    public async Task<List<ClientOutput>?> Execute(Guid companyId)
    {
        var result = await _context.Clients.
                     Include(x => x.Companies).
                     AsNoTracking().
                     Where(x => x.CompanyId == companyId && x.Status == true).
                     ToListAsync();

        var output = result.Adapt<List<ClientOutput>>();

        return output;
    }
}