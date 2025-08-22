using Microsoft.AspNetCore.ResponseCompression;
using Orquestra.API.Extensions;
using Orquestra.API.Filters;
using Orquestra.Infrastructure.Serialization;
using System.IO.Compression;
using System.Text.Json.Serialization;

namespace Orquestra.API;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjectionAPI(this IServiceCollection services, WebApplicationBuilder builder)
    {
        IWebHostEnvironment env = builder.Environment;

        AddCompression(services);
        AddControllers(services, env);
        AddMisc(services);

        return services;
    }

    private static void AddCompression(IServiceCollection services)
    {
        services.AddResponseCompression(x =>
        {
            x.EnableForHttps = true;
            x.Providers.Add<BrotliCompressionProvider>();
            x.Providers.Add<GzipCompressionProvider>();
        });

        services.Configure<BrotliCompressionProviderOptions>(x =>
        {
            x.Level = CompressionLevel.Optimal;
        });

        services.Configure<GzipCompressionProviderOptions>(x =>
        {
            x.Level = CompressionLevel.Optimal;
        });
    }

    private static void AddControllers(IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddControllers(x =>
        {
            x.Filters.Add<ErrorFilter>();
        }).
           AddJsonOptions(x =>
           {
               x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
               x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
               x.JsonSerializerOptions.WriteIndented = env.IsDevelopment();
               x.JsonSerializerOptions.Converters.Add(new BrasiliaDateTimeConverter());
           });
    }

    private static void AddMisc(IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddResponseCaching();

        services.AddHttpContextAccessor(); // Serviço necessário para habilitar o IHttpContextAccessor em Infrastructure/Context;

        services.AddUserRateLimiting();
    }
}