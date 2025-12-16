using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.ServiceOrders.Create;
using Orquestra.Application.UseCases.ServiceOrders.Delete;
using Orquestra.Application.UseCases.ServiceOrders.GetAllByCompanyId;
using Orquestra.Application.UseCases.ServiceOrders.Update;

namespace Orquestra.Application.UseCases.ServiceOrders;

public static class DependencyInjection
{
    public static IServiceCollection AddServiceOrdersApplication(this IServiceCollection services)
    {
        services.AddScoped<IGetAllServiceOrderByCompanyId, GetAllServiceOrderByCompanyId>();
        services.AddScoped<ICreateServiceOrder, CreateServiceOrder>();
        services.AddScoped<IUpdateServiceOrder, UpdateServiceOrder>();
        services.AddScoped<IDeleteServiceOrder, DeleteServiceOrder>();

        return services;
    }
}