using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.Locations.Cities.Get;
using Orquestra.Application.UseCases.Locations.States.Get;

namespace Orquestra.Application.UseCases.Locations;

public static class DependencyInjection
{
    public static IServiceCollection AddLocationsApplication(this IServiceCollection services)
    {
        services.AddScoped<IGetState, GetState>();
        services.AddScoped<IGetCity, GetCity>();

        return services;
    }
}