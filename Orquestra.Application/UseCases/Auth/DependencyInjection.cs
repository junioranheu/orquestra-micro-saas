using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.Auth.CreateRefreshTokenJWT;
using Orquestra.Application.UseCases.Auth.CreateTokenJWT;
using Orquestra.Application.UseCases.Auth.GetMe;
using Orquestra.Application.UseCases.Auth.GetRefreshTokenJWT;
using Orquestra.Application.UseCases.Auth.Logout;

namespace Orquestra.Application.UseCases.Auth;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateToken, CreateToken>();
        services.AddScoped<ICreateRefreshToken, CreateRefreshToken>();
        services.AddScoped<IGetRefreshToken, GetRefreshToken>();
        services.AddScoped<ILogoutUser, LogoutUser>();
        services.AddScoped<IGetMeOutput, GetMeOutput>();

        return services;
    }
}