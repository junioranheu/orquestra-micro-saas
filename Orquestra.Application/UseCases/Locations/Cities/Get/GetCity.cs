using Microsoft.EntityFrameworkCore;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Locations.Cities.Get;

public sealed class GetCity(Context context) : IGetCity
{
    private readonly Context _context = context;

    public async Task<List<LocationCity>?> Execute(int? locationStateId = null)
    {
        var result = await _context.LocationCities.
                     Include(x => x.LocationState).
                     AsNoTracking().
                     Where(x => 
                        ((locationStateId <= 0 || locationStateId == null) || x.LocationStateId == locationStateId) && 
                        x.Status == true
                     ).ToListAsync();

        return result;
    }
}