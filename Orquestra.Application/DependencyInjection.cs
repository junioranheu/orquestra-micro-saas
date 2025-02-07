using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orquestra.Application.AutoMapper;
using Orquestra.Application.UseCases.Auth;
using Orquestra.Application.UseCases.Companies;
using Orquestra.Application.UseCases.CompanyUsers;
using Orquestra.Application.UseCases.Locations;
using Orquestra.Application.UseCases.Logs;
using Orquestra.Application.UseCases.Users;

namespace Orquestra.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjectionApplication(this IServiceCollection services, WebApplicationBuilder builder)
    {
        AddAutoMapper(services);
        AddLogger(builder);

        AddUseCases(services);
        AddServices(services);

        return services;
    }

    private static void AddAutoMapper(IServiceCollection services)
    {
        var mapperConfig = new MapperConfiguration(x =>
        {
            x.AddProfile(new AutoMapperConfig());
        });

        IMapper mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper);
    }

    private static void AddLogger(WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
    }

    private static void AddUseCases(IServiceCollection services)
    {
        services.AddLogsApplication();
        services.AddLocationsApplication();
        services.AddUsersApplication();
        services.AddAuthApplication();
        services.AddCompaniesApplication();
        services.AddCompanyUsersApplication();
    }

    private static void AddServices(IServiceCollection services)
    {

    }
}