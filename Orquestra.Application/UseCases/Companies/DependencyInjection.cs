using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.Companies.Base;
using Orquestra.Application.UseCases.Companies.Create;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.Companies.ResendVerifyEmail;
using Orquestra.Application.UseCases.Companies.Update;
using Orquestra.Application.UseCases.Companies.UpdatePlanType;
using Orquestra.Application.UseCases.Companies.Verify;
using Orquestra.Application.UseCases.CompanyInvoices.Create;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.Invite;
using Orquestra.Application.UseCases.CompanyUsers.UpdateCurrentMain;
using Orquestra.Application.UseCases.Integrations.WhatsApp.Create;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Verifications.Create;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Messaging.Publishers;
using Orquestra.Infrastructure.Services.Env;

namespace Orquestra.Application.UseCases.Companies;

public static class DependencyInjection
{
    public static IServiceCollection AddCompaniesApplication(this IServiceCollection services)
    {
        services.AddScoped<IGetCompany, GetCompany>();
        services.AddScoped<ICreateCompany, CreateCompany>();
        services.AddScoped<IUpdateCompany, UpdateCompany>();
        services.AddScoped<IVerifyCompany, VerifyCompany>();
        services.AddScoped<IUpdatePlanTypeCompany, UpdatePlanTypeCompany>();
        services.AddScoped<IResendVerifyEmailCompany, ResendVerifyEmailCompany>();
     
        services.AddScoped(x => new CompanyBaseDependencies(
           x.GetRequiredService<Context>(),
           x.GetRequiredService<IEnvService>(),
           x.GetRequiredService<ICreateVerification>(),
           x.GetRequiredService<IInviteCompanyUser>(),
           x.GetRequiredService<IUpdateCurrentMainCompanyUser>(),
           x.GetRequiredService<IGetUser>(),
           x.GetRequiredService<ICheckIfUserIsLinkedCompanyUser>(),
           x.GetRequiredService<ICreateCompanyInvoice>(),
           x.GetRequiredService<ICreateIntegrationWhatsApp>(),
           x.GetRequiredService<IGenericPublisher>()
        ));

        return services;
    }
}