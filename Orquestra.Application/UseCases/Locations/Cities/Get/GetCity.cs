using Microsoft.EntityFrameworkCore;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Locations.Cities.Get;

public sealed class GetCity(Context context) : IGetCity
{
    private readonly Context _context = context;

    public async Task<List<LocationCity>?> Execute()
    {
        var result = await _context.LocationCities.
                     Include(x => x.LocationStates).
                     AsNoTracking().
                     Where(x => x.Status == true).
                     ToListAsync();

        return result;
    }
}