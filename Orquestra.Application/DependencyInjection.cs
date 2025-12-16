using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orquestra.Application.UseCases.Auth;
using Orquestra.Application.UseCases.Clients;
using Orquestra.Application.UseCases.ClientsFollowUps;
using Orquestra.Application.UseCases.Companies;
using Orquestra.Application.UseCases.CompanyInvoices;
using Orquestra.Application.UseCases.CompanyUsers;
using Orquestra.Application.UseCases.Integrations;
using Orquestra.Application.UseCases.Integrations.WhatsApp.SendMessageBatch;
using Orquestra.Application.UseCases.Inventories;
using Orquestra.Application.UseCases.Locations;
using Orquestra.Application.UseCases.Logs;
using Orquestra.Application.UseCases.Quotes;
using Orquestra.Application.UseCases.Sales;
using Orquestra.Application.UseCases.Schedules;
using Orquestra.Application.UseCases.ServiceOrders;
using Orquestra.Application.UseCases.Users;
using Orquestra.Application.UseCases.Verifications;
using Orquestra.Infrastructure.Jobs.Base.Handlers;

namespace Orquestra.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjectionApplication(this IServiceCollection services, WebApplicationBuilder builder)
    {
        AddLogger(builder);
        AddUseCases(services);
        AddServices(services);
        AddHandlers(services);

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
        services.AddVerificationsApplication();
        services.AddCompanyInvoicesApplication();
        services.AddIntegrationsApplication();
        services.AddInventoriesApplication();
        services.AddClientsFollowUpsApplication();
        services.AddQuotesApplication();
        services.AddSalesApplication();
        services.AddServiceOrdersApplication();
    }

    private static void AddServices(IServiceCollection _)
    {

    }

    private static void AddHandlers(IServiceCollection services)
    {
        services.AddScoped<ISendMessageBatchWhatsAppHandler, SendMessageBatchWhatsAppHandler>();
    }
}