using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.CompanyUsers.Invite;
using Orquestra.Application.UseCases.CompanyUsers.UpdateCurrentMainCompany;
using Orquestra.Application.UseCases.CompanyUsers.Verify;

namespace Orquestra.Application.UseCases.CompanyUsers;

public static class DependencyInjection
{
    public static IServiceCollection AddCompanyUsersApplication(this IServiceCollection services)
    {
        services.AddScoped<IGetCompanyUserByCompanyId, GetCompanyUserByCompanyId>();
        services.AddScoped<ICheckIfUserIsLinkedCompanyUser, CheckIfUserIsLinkedCompanyUser>();
        services.AddScoped<IVerifyCompanyUser, VerifyCompanyUser>();
        services.AddScoped<IUpdateCurrentMainCompanyUser, UpdateCurrentMainCompanyUser>();
        services.AddScoped<IInviteCompanyUser, InviteCompanyUser>();

        return services;
    }
}