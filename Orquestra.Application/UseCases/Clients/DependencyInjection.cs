using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.Clients.Create;
using Orquestra.Application.UseCases.Clients.Delete;
using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Clients.GetAllByCompanyId;
using Orquestra.Application.UseCases.Clients.Update;

namespace Orquestra.Application.UseCases.Clients;

public static class DependencyInjection
{
    public static IServiceCollection AddClientsApplication(this IServiceCollection services)
    {
        services.AddScoped<IGetClient, GetClient>();
        services.AddScoped<IGetAllClientByCompanyId, GetAllClientByCompanyId>();
        services.AddScoped<ICreateClient, CreateClient>();
        services.AddScoped<IUpdateClient, UpdateClient>();
        services.AddScoped<IDeleteClient, DeleteClient>();

        return services;
    }
}