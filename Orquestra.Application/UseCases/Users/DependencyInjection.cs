using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.Users.Create;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Users.RecoverPassword;
using Orquestra.Application.UseCases.Users.Update;
using Orquestra.Application.UseCases.Users.Verify;

namespace Orquestra.Application.UseCases.Users;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersApplication(this IServiceCollection services)
    {
        services.AddScoped<IGetUser, GetUser>();
        services.AddScoped<ICreateUser, CreateUser>();
        services.AddScoped<IUpdateUser, UpdateUser>();
        services.AddScoped<IVerifyUser, VerifyUser>();
        services.AddScoped<IRecoverPasswordUser, RecoverPasswordUser>();

        return services;
    }
}