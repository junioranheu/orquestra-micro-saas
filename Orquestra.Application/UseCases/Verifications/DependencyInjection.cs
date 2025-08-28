using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.Verifications.Create;
using Orquestra.Application.UseCases.Verifications.Get;
using Orquestra.Application.UseCases.Verifications.Update;

namespace Orquestra.Application.UseCases.Verifications;

public static class DependencyInjection
{
    public static IServiceCollection AddVerificationsApplication(this IServiceCollection services)
    {
        services.AddScoped<IGetVerification, GetVerification>();
        services.AddScoped<ICreateVerification, CreateVerification>();
        services.AddScoped<IUpdateVerification, UpdateVerification>();

        return services;
    }
}