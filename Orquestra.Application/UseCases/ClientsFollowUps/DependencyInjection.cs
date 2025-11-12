using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.ClientsFollowUps.Create;
using Orquestra.Application.UseCases.ClientsFollowUps.Delete;
using Orquestra.Application.UseCases.ClientsFollowUps.Get;
using Orquestra.Application.UseCases.ClientsFollowUps.Update;

namespace Orquestra.Application.UseCases.ClientsFollowUps;

public static class DependencyInjection
{
    public static IServiceCollection AddClientsFollowUpsApplication(this IServiceCollection services)
    {
        services.AddScoped<IGetClientFollowUp, GetClientFollowUp>();
        services.AddScoped<ICreateClientFollowUp, CreateClientFollowUp>();
        services.AddScoped<IUpdateClientFollowUp, UpdateClientFollowUp>();
        services.AddScoped<IDeleteClientFollowUp, DeleteClientFollowUp>();

        return services;
    }
}