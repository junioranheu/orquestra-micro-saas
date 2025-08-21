using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.Companies.Create;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.Companies.Verify;

namespace Orquestra.Application.UseCases.Companies;

public static class DependencyInjection
{
    public static IServiceCollection AddCompaniesApplication(this IServiceCollection services)
    {
        services.AddScoped<IGetCompany, GetCompany>();
        services.AddScoped<ICreateCompany, CreateCompany>();
        services.AddScoped<IVerifyCompany, VerifyCompany>();

        return services;
    }
}