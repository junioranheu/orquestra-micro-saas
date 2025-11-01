using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Integrations.WhatsApp.Base;
using Orquestra.Application.UseCases.Integrations.WhatsApp.Create;
using Orquestra.Application.UseCases.Integrations.WhatsApp.Get;
using Orquestra.Application.UseCases.Integrations.WhatsApp.SendMessageBatch;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Services.Sms;

namespace Orquestra.Application.UseCases.Integrations;

public static class DependencyInjection
{
    public static IServiceCollection AddIntegrationsApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateIntegrationWhatsApp, CreateIntegrationWhatsApp>();
        services.AddScoped<IGetIntegrationWhatsApp, GetIntegrationWhatsApp>();
        services.AddScoped<ISendMessageBatchWhatsApp, SendMessageBatchWhatsApp>();

        services.AddScoped(x => new IntegrationWhatsAppBaseDependencies(
           x.GetRequiredService<Context>(),
           x.GetRequiredService<ICheckIfUserIsLinkedCompanyUser>(),
           x.GetRequiredService<ISmsService>()
        ));

        return services;
    }
}