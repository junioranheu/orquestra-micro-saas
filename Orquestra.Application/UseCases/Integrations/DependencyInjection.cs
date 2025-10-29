using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetCurrentMain;
using Orquestra.Application.UseCases.Integrations.WhatsApp.Base;
using Orquestra.Application.UseCases.Integrations.WhatsApp.Create;
using Orquestra.Application.UseCases.Integrations.WhatsApp.SendMessage;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Integrations;

public static class DependencyInjection
{
    public static IServiceCollection AddIntegrationsApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateIntegrationWhatsApp, CreateIntegrationWhatsApp>();
        services.AddScoped<ISendMessageWhatsApp, SendMessageWhatsApp>();

        services.AddScoped(x => new IntegrationWhatsAppBaseDependencies(
           x.GetRequiredService<Context>(),
           x.GetRequiredService<ICheckIfUserIsLinkedCompanyUser>(),
           x.GetRequiredService<IGetCurrentMainCompanyUser>()   
        ));

        return services;
    }
}