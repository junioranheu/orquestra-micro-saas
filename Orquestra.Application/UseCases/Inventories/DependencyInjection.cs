using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.Inventories.Create;
using Orquestra.Application.UseCases.Inventories.Delete;
using Orquestra.Application.UseCases.Inventories.GetAllByCompanyId;
using Orquestra.Application.UseCases.Inventories.Update;

namespace Orquestra.Application.UseCases.Inventories;

public static class DependencyInjection
{
    public static IServiceCollection AddInventoriesApplication(this IServiceCollection services)
    {
        services.AddScoped<IGetAllInventoryByCompanyId, GetAllInventoryByCompanyId>();
        services.AddScoped<ICreateInventory, CreateInventory>();
        services.AddScoped<IUpdateInventory, UpdateInventory>();
        services.AddScoped<IDeleteInventory, DeleteInventory>();

        return services;
    }
}