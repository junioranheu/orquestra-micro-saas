using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Clients.Get;

public sealed class GetClient(Context context) : IGetClient
{
    private readonly Context _context = context;

    public async Task<ClientOutput?> Execute(Guid clientId)
    {
        var result = await _context.Clients.
                     Include(x => x.Company).
                     AsNoTracking().
                     Where(x =>
                        x.Status == true &&
                        x.ClientId == clientId
                     ).
                     FirstOrDefaultAsync();

        var output = result.Adapt<ClientOutput>();

        return output;
    }
}