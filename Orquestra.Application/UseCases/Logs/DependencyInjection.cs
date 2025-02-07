using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.Logs.Create;
using Orquestra.Application.UseCases.Logs.Get;

namespace Orquestra.Application.UseCases.Logs;

public static class DependencyInjection
{
    public static IServiceCollection AddLogsApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateLog, CreateLog>();
        services.AddScoped<IGetLog, GetLog>();

        return services;
    }
}