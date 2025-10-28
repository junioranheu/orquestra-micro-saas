using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.Logs.Create;
using Orquestra.Application.UseCases.Logs.Get;
using Orquestra.Application.UseCases.Logs.GetNotification;

namespace Orquestra.Application.UseCases.Logs;

public static class DependencyInjection
{
    public static IServiceCollection AddLogsApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateLog, CreateLog>();
        services.AddScoped<IGetLog, GetLog>();
        services.AddScoped<IGetNotificationLog, GetNotificationLog>();

        return services;
    }
}