using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.CompanyInvoices.Create;
using Orquestra.Application.UseCases.CompanyInvoices.Get;
using Orquestra.Application.UseCases.CompanyInvoices.Pay;

namespace Orquestra.Application.UseCases.CompanyInvoices;

public static class DependencyInjection
{
    public static IServiceCollection AddCompanyInvoicesApplication(this IServiceCollection services)
    {
        services.AddScoped<IGetCompanyInvoice, GetCompanyInvoice>();
        services.AddScoped<ICreateCompanyInvoice, CreateCompanyInvoice>();
        services.AddScoped<IPayCompanyInvoice, PayCompanyInvoice>();
  
        return services;
    }
}