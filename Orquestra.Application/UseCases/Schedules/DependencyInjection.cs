using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.Schedules.Create;
using Orquestra.Application.UseCases.Schedules.Get;
using Orquestra.Application.UseCases.Schedules.Update;

namespace Orquestra.Application.UseCases.Schedules;

public static class DependencyInjection
{
    public static IServiceCollection AddSchedulesApplication(this IServiceCollection services)
    {
        services.AddScoped<IGetSchedule, GetSchedule>();
        services.AddScoped<ICreateSchedule, CreateSchedule>();
        services.AddScoped<IUpdateSchedule, UpdateSchedule>();

        return services;
    }
}