using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.Sales.GetChart;

namespace Orquestra.Application.UseCases.Sales;

public static class DependencyInjection
{
    public static IServiceCollection AddSalesApplication(this IServiceCollection services)
    {
        services.AddScoped<IGetChartSales, GetChartSales>();

        return services;
    }
}