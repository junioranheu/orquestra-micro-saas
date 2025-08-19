using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Schedules.Base;
using Orquestra.Application.UseCases.Schedules.Create;
using Orquestra.Application.UseCases.Schedules.Get;
using Orquestra.Application.UseCases.Schedules.GetAllByCompanyId;
using Orquestra.Application.UseCases.Schedules.Update;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Schedules;

public static class DependencyInjection
{
    public static IServiceCollection AddSchedulesApplication(this IServiceCollection services)
    {
        services.AddScoped<IGetSchedule, GetSchedule>();
        services.AddScoped<IGetScheduleByCompanyId, GetScheduleByCompanyId>();
        services.AddScoped<ICreateSchedule, CreateSchedule>();
        services.AddScoped<IUpdateSchedule, UpdateSchedule>();

        services.AddScoped(x => new ScheduleBaseDependencies(
           x.GetRequiredService<Context>(),
           x.GetRequiredService<ICheckIfUserIsLinkedCompanyUser>(),
           x.GetRequiredService<IGetClient>(),
           x.GetRequiredService<IGetCompany>()
        ));

        return services;
    }
}