using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.Users.Create;
using Orquestra.Application.UseCases.Users.GetByUserNameOrEmail;

namespace Orquestra.Application.UseCases.Users;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersApplication(this IServiceCollection services)
    {
        services.AddScoped<IGetUserByUserNameOrEmail, GetUserByUserNameOrEmail>();
        services.AddScoped<ICreateUser, CreateUser>();

        return services;
    }
}