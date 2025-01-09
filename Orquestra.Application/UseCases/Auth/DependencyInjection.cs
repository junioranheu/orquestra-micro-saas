using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.Auth.CreateRefreshTokenJWT;
using Orquestra.Application.UseCases.Auth.CreateTokenJWT;

namespace Orquestra.Application.UseCases.Auth;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateToken, CreateToken>();
        services.AddScoped<ICreateRefreshToken, CreateRefreshToken>();

        return services;
    }
}