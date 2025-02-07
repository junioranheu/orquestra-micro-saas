using Orquestra.Domain.Entities;

namespace Orquestra.Application.UseCases.Locations.States.Get;

public interface IGetState
{
    Task<List<LocationState>?> Execute();
}