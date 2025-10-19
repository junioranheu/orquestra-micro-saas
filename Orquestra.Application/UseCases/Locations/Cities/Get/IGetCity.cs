using Orquestra.Domain.Entities;

namespace Orquestra.Application.UseCases.Locations.Cities.Get;

public interface IGetCity
{
    Task<List<LocationCity>?> Execute(int? locationStateId = null);
}