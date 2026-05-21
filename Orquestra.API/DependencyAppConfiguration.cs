using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Orquestra.API.Middlewares;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Seed;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Diagnostics;
using System.Text.Json;

namespace Orquestra.API;

public static class DependencyAppConfiguration
{
    public static async Task<WebApplication> UseAppConfiguration(this WebApplication app, WebApplicationBuilder builder)
    {
        AddMiddleware(app);
        AddSwagger(app);
        AddHttpsRedirection(app);
        AddCors(app, builder);
        AddCompression(app);
        AddAuth(app);
        AddObservability(app);
        AddCaching(app);
        AddRateLimiting(app);
        AddDeveloperExceptionPage(app);
        AddHealthCheck(app);
        await HandleDbInitialize(app);

        return app;
    }

    private static void AddMiddleware(WebApplication app)
    {
        app.UseMiddleware<TokenRefreshMiddleware>(); // Esse middleware deve obrigatoriamente vir antes de AddAuth();
        app.UseMiddleware<CsrfOriginMiddleware>(); // ITEM 2/9: Validação de Origin para proteção CSRF;
    }

    private static void AddSwagger(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", SystemConsts.App.NameApi);
                c.DocExpansion(DocExpansion.None);

                if (OperatingSystem.IsMacOS() || OperatingSystem.IsWindows())
                {
                    c.RoutePrefix = string.Empty;
                }
            });
        }
    }

    private static void AddHttpsRedirection(WebApplication app)
    {
        if (app.Environment.IsProduction())
        {
            app.UseHttpsRedirection();
        }
    }

    private static void AddCors(WebApplication app, WebApplicationBuilder builder)
    {
        app.UseCors(builder.Configuration["CORSSettings:Cors"] ?? string.Empty);
    }

    private static void AddCompression(WebApplication app)
    {
        /// <summary>
        /// O trecho "app.UseWhen" abaixo é necessário quando a API tem uma resposta IAsyncEnumerable/Yield;
        /// O "UseResponseCompression" conflita com esse tipo de requisição, portanto é obrigatória a verificação abaixo;
        /// Caso não existam requisições desse tipo na API, é apenas necessário o trecho "app.UseResponseCompression()";
        /// </summary>
        app.UseWhen(context => !IsStreamingRequest(context), x =>
        {
            x.UseResponseCompression();
        });

        static bool IsStreamingRequest(HttpContext context)
        {
            Endpoint? endpoint = context.GetEndpoint();

            if (endpoint is RouteEndpoint routeEndpoint)
            {
                ControllerActionDescriptor? action = routeEndpoint.Metadata.GetMetadata<ControllerActionDescriptor>();

                if (action is not null)
                {
                    Type? tipo = action.MethodInfo.ReturnType;

                    if (tipo.IsGenericType && tipo.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>))
                    {
                        return true;
                    }

                    return false;
                }
            }

            return false;
        }
    }

    private static void AddAuth(WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }

    private static void AddObservability(WebApplication app)
    {
        ActivitySource activitySource = new(SystemConsts.App.NameApi);

        ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();

        ActivityListener listener = new()
        {
            ShouldListenTo = source => source.Name == "Microsoft.AspNetCore" || source.Name == SystemConsts.App.NameApi,
            Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity =>
            {
                // Filtra métodos irrelevantes;
                string? method = activity.Tags.FirstOrDefault(t => t.Key == "http.request.method").Value?.ToString();
                string? path = activity.Tags.FirstOrDefault(t => t.Key == "http.route").Value?.ToString();

                if (method == "OPTIONS" || string.IsNullOrEmpty(path) || path.Contains("favicon") || path.Contains("health"))
                {
                    return;
                }

                string? statusCodeStr = activity.GetTagItem("http.response.status_code")?.ToString();

                logger.LogInformation("[Observability] {Tags}, [duration, {Duration}ms], [status, {Status}]",
                    string.Join(", ", activity.Tags.Where(x => x.Key.StartsWith("http") || x.Key == "server.address")),
                    activity.Duration.TotalMilliseconds,
                    statusCodeStr
                );
            }
        };

        ActivitySource.AddActivityListener(listener);
    }

    private static void AddCaching(WebApplication app)
    {
        app.UseResponseCaching();
    }

    private static void AddRateLimiting(WebApplication app)
    {
        app.UseRateLimiter();
    }

    private static void AddDeveloperExceptionPage(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
    }

    private static void AddHealthCheck(WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";

                string result = JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(x => new {
                        name = x.Key,
                        status = x.Value.Status.ToString(),
                        description = x.Value.Description
                    })
                });

                await context.Response.WriteAsync(result);
            }
        });
    }

    private static async Task HandleDbInitialize(WebApplication app)
    {
        bool isApplyReset = false;
        bool isApplyMigrations = false;
        bool isApplySeed = false;

        if (!isApplyReset && !isApplyMigrations && !isApplySeed)
        {
            return;
        }

        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;
        Context context = services.GetRequiredService<Context>();

        await DbInitializer.Initialize(context, isDev: app.Environment.IsDevelopment(), isApplyReset, isApplyMigrations, isApplySeed);
    }
}