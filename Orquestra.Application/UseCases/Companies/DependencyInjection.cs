using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.Companies.Create;

namespace Orquestra.Application.UseCases.Companies;

public static class DependencyInjection
{
    public static IServiceCollection AddCompaniesApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateCompany, CreateCompany>();

        return services;
    }
}