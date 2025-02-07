using Microsoft.EntityFrameworkCore;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Locations.States.Get;


public sealed class GetState(Context context) : IGetState
{
    private readonly Context _context = context;

    public async Task<List<LocationState>?> Execute()
    {
        var result = await _context.LocationStates.AsNoTracking().ToListAsync();

        return result;
    }
}