using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetCurrentMain;
using Orquestra.Application.UseCases.Integrations.Whatsapp.Base;
using Orquestra.Application.UseCases.Integrations.Whatsapp.SendMessage;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Integrations;

public static class DependencyInjection
{
    public static IServiceCollection AddIntegrationsApplication(this IServiceCollection services)
    {
        services.AddScoped<ISendMessageWhatsapp, SendMessageWhatsapp>();

        services.AddScoped(x => new IntegrationWhatsappBaseDependencies(
           x.GetRequiredService<Context>(),
           x.GetRequiredService<ICheckIfUserIsLinkedCompanyUser>(),
           x.GetRequiredService<IGetCurrentMainCompanyUser>()   
        ));

        return services;
    }
}