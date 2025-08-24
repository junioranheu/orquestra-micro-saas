using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.CreateRange;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.CompanyUsers.UpdateCurrentMainCompany;
using Orquestra.Application.UseCases.CompanyUsers.Verify;

namespace Orquestra.Application.UseCases.CompanyUsers;

public static class DependencyInjection
{
    public static IServiceCollection AddCompanyUsersApplication(this IServiceCollection services)
    {
        services.AddScoped<IGetCompanyUserByCompanyId, GetCompanyUserByCompanyId>();
        services.AddScoped<ICreateRangeCompanyUser, CreateRangeCompanyUser>();
        services.AddScoped<ICheckIfUserIsLinkedCompanyUser, CheckIfUserIsLinkedCompanyUser>();
        services.AddScoped<IVerifyCompanyUser, VerifyCompanyUser>();
        services.AddScoped<IUpdateCurrentMainCompanyUser, UpdateCurrentMainCompanyUser>();

        return services;
    }
}