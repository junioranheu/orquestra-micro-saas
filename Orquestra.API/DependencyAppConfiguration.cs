using Microsoft.AspNetCore.Mvc.Controllers;
using Orquestra.API.Middlewares;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Seed;
using Swashbuckle.AspNetCore.SwaggerUI;

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
        AddMisc(app);
        await HandleDbInitialize(app);

        return app;
    }

    private static void AddMiddleware(WebApplication app)
    {
        app.UseMiddleware<TokenRefreshMiddleware>(); // Esse middleware deve obrigatoriamente vir antes de AddAuth();
    }

    private static void AddSwagger(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", SystemConsts.NameApi);
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

    private static void AddMisc(WebApplication app)
    {
        app.UseResponseCaching();
        app.UseRateLimiter();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
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

        await DbInitializer.Initialize(context, isApplyReset, isApplyMigrations, isApplySeed);
    }
}