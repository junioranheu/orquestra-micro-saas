using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Clients.Get;

public sealed class GetClient(Context context, IMapper map) : IGetClient
{
    private readonly Context _context = context;
    private readonly IMapper _map = map;

    public async Task<ClientOutput?> Execute(Guid clientId)
    {
        var result = await _context.Clients.
                     Include(x => x.Companies).
                     AsNoTracking().
                     Where(x =>
                        x.Status == true &&
                        x.ClientId == clientId
                     ).
                     FirstOrDefaultAsync();

        var output = _map.Map<ClientOutput>(result);

        return output;
    }

    public async Task<List<ClientOutput>?> GetAll(Guid companyId)
    {
        var result = await _context.Clients.
                     Include(x => x.Companies).
                     AsNoTracking().
                     Where(x => x.CompanyId == companyId && x.Status == true).
                     ToListAsync();

        var output = _map.Map<List<ClientOutput>>(result);

        return output;
    }
}