using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orquestra.Application.UseCases.Auth;
using Orquestra.Application.UseCases.Clients;
using Orquestra.Application.UseCases.Companies;
using Orquestra.Application.UseCases.CompanyUsers;
using Orquestra.Application.UseCases.Locations;
using Orquestra.Application.UseCases.Logs;
using Orquestra.Application.UseCases.Schedules;
using Orquestra.Application.UseCases.Users;

namespace Orquestra.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjectionApplication(this IServiceCollection services, WebApplicationBuilder builder)
    {
        AddLogger(builder);
        AddUseCases(services);
        AddServices(services);

        return services;
    }

    private static void AddLogger(WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddAzureWebAppDiagnostics();
    }

    private static void AddUseCases(IServiceCollection services)
    {
        services.AddLogsApplication();
        services.AddLocationsApplication();
        services.AddUsersApplication();
        services.AddAuthApplication();
        services.AddCompaniesApplication();
        services.AddCompanyUsersApplication();
        services.AddSchedulesApplication();
        services.AddClientsApplication();
    }

    private static void AddServices(IServiceCollection _)
    {

    }
}