using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.CompanyUsers.CreateRange;

namespace Orquestra.Application.UseCases.CompanyUsers;

public static class DependencyInjection
{
    public static IServiceCollection AddCompanyUsersApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateRangeCompanyUser, CreateRangeCompanyUser>();

        return services;
    }
}