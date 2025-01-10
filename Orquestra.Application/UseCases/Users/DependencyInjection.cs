using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.Users.Create;
using Orquestra.Application.UseCases.Users.GetByEmail;

namespace Orquestra.Application.UseCases.Users;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersApplication(this IServiceCollection services)
    {
        services.AddScoped<IGetUserByEmail, GetUserByEmail>();
        services.AddScoped<ICreateUser, CreateUser>();

        return services;
    }
}