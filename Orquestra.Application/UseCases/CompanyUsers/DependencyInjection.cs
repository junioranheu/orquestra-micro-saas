using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUser;
using Orquestra.Application.UseCases.CompanyUsers.CreateRange;
using Orquestra.Application.UseCases.CompanyUsers.Get;

namespace Orquestra.Application.UseCases.CompanyUsers;

public static class DependencyInjection
{
    public static IServiceCollection AddCompanyUsersApplication(this IServiceCollection services)
    {
        services.AddScoped<IGetCompanyUser, GetCompanyUser>();
        services.AddScoped<ICreateRangeCompanyUser, CreateRangeCompanyUser>();
        services.AddScoped<ICheckIfUserIsLinkedCompanyUser, CheckIfUserIsLinkedCompanyUser>();

        return services;
    }
}