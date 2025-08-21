using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Orquestra.API.Extensions;
using Orquestra.API.Filters;
using System.IO.Compression;
using System.Text.Json.Serialization;

namespace Orquestra.API;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjectionAPI(this IServiceCollection services, WebApplicationBuilder builder)
    {
        IWebHostEnvironment env = builder.Environment;

        // AddKestrel(services);
        AddCompression(services);
        AddControllers(services, env);
        AddMisc(services);

        return services;
    }

    private static void AddKestrel(IServiceCollection services)
    {
        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(30);
        });
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