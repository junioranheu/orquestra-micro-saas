using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.Delete;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.CompanyUsers.GetCurrentMain;
using Orquestra.Application.UseCases.CompanyUsers.Invite;
using Orquestra.Application.UseCases.CompanyUsers.Update;
using Orquestra.Application.UseCases.CompanyUsers.UpdateCurrentMain;
using Orquestra.Application.UseCases.CompanyUsers.Verify;

namespace Orquestra.Application.UseCases.CompanyUsers;

public static class DependencyInjection
{
    public static IServiceCollection AddCompanyUsersApplication(this IServiceCollection services)
    {
        services.AddScoped<IGetAllCompanyUserByCompanyId, GetAllCompanyUserByCompanyId>();
        services.AddScoped<ICheckIfUserIsLinkedCompanyUser, CheckIfUserIsLinkedCompanyUser>();
        services.AddScoped<IVerifyCompanyUser, VerifyCompanyUser>();
        services.AddScoped<IUpdateCurrentMainCompanyUser, UpdateCurrentMainCompanyUser>();
        services.AddScoped<IGetCurrentMainCompanyUser, GetCurrentMainCompanyUser>();
        services.AddScoped<IInviteCompanyUser, InviteCompanyUser>();
        services.AddScoped<IUpdateCompanyUser, UpdateCompanyUser>();
        services.AddScoped<IDeleteCompanyUser, DeleteCompanyUser>();

        return services;
    }
}