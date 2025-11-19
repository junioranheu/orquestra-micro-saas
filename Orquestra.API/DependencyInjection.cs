using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Orquestra.API.Extensions;
using Orquestra.API.Filters;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure.Factory.DataBase;
using Orquestra.Infrastructure.Serialization;
using System.IO.Compression;
using System.Text.Json.Serialization;

namespace Orquestra.API;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjectionAPI(this IServiceCollection services, WebApplicationBuilder builder)
    {
        IWebHostEnvironment env = builder.Environment;

        AddSwagger(services);
        AddCors(services, builder);
        AddCompression(services);
        AddControllers(services, env);
        AddObservability(services);
        AddCaching(services);
        AddHttpContextAccessor(services);
        AddRateLimiting(services);
        AddHealthCheck(services, builder);

        return services;
    }

    private static void AddSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new()
            {
                Title = SystemConsts.App.NameApi,
                Version = "v1"
            });
        });
    }

    private static void AddCors(IServiceCollection services, WebApplicationBuilder builder)
    {
        string[] frontendUrls =
        [
            builder.Configuration["Urls:Development:Frontend"] ?? string.Empty,
            builder.Configuration["Urls:Production:Frontend"] ?? string.Empty,
            builder.Configuration["Urls:Production:Frontend_2"] ?? string.Empty
        ];

        if (frontendUrls is null || frontendUrls.Any(x => string.IsNullOrEmpty(x)))
        {
            throw new InvalidOperationException("Erro interno crítico: um ou mais URLs de Frontend não estão configurados no appsettings.json.");
        }

        services.AddCors(x =>
            x.AddPolicy(name: builder.Configuration["CORSSettings:Cors"] ?? string.Empty, builder =>
            {
                #region obsoleto
                //builder.AllowAnyHeader().
                //        AllowAnyMethod().
                //        SetIsOriginAllowed((host) => true).
                //        AllowCredentials();
                #endregion

                builder.WithOrigins(frontendUrls).
                        AllowAnyHeader().
                        AllowAnyMethod().
                        AllowCredentials();
            })
        );
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
            // x.Filters.Add<RequestFilter>(); // Substituído pelo ChangeLogInterceptor;
        }).
           AddJsonOptions(x =>
           {
               x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
               // x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); // Exibir, por exemplo, "ClinicaOdontologia" em vez de "1";
               x.JsonSerializerOptions.WriteIndented = env.IsDevelopment();
               x.JsonSerializerOptions.Converters.Add(new BrasiliaDateTimeConverter());
           });
    }

    private static void AddObservability(IServiceCollection services)
    {
        /// Foi necessário instalar estas seguintes dependências:
        /// OpenTelemetry;
        /// OpenTelemetry.Extensions.Hosting;
        /// OpenTelemetry.Instrumentation.AspNetCore;
        services.AddOpenTelemetry().
            ConfigureResource(resource => resource.AddService(SystemConsts.App.NameApi)).
            WithTracing(tracing => tracing.
                AddAspNetCoreInstrumentation()
            );
    }

    private static void AddCaching(IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddResponseCaching();
    }

    private static void AddHttpContextAccessor(IServiceCollection services)
    {
        services.AddHttpContextAccessor(); // Serviço necessário para habilitar o IHttpContextAccessor em Infrastructure/Context;
    }

    private static void AddRateLimiting(IServiceCollection services)
    {
        services.AddUserRateLimiting();
    }

    private static void AddHealthCheck(IServiceCollection services, WebApplicationBuilder builder)
    {
        DataBaseConnection connection = new(builder.Configuration);
        string con = connection.GetConnectionString();
        string type = connection.GetConnectionTypeName();

        string front = builder.Configuration["Urls:Production:Frontend"] ?? string.Empty;
        Uri frontUri = new(front);
        string domain = frontUri.Host;

        services.AddHealthChecks().
            AddCheck($"Banco de dados {type}", () =>
            {
                try
                {
                    using NpgsqlConnection connection = new(con);
                    connection.Open();

                    return HealthCheckResult.Healthy("Banco de dados conectado com sucesso.");
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Unhealthy($"Falha ao conectar ao banco de dados: {ex.Message}");
                }
            }).
            AddCheck($"Front-end {domain}", () =>
            {
                try
                {
                    using HttpClient client = new();
                    HttpResponseMessage response = client.GetAsync(front).Result;

                    return response.IsSuccessStatusCode ? HealthCheckResult.Healthy("Front-end conectado com sucesso.") : HealthCheckResult.Unhealthy("Front-end indisponível");
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Unhealthy($"Erro ao acessar front-end: {ex.Message}");
                }
            });
    }
}