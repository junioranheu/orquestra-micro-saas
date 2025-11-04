using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure.Auth.Models;
using Orquestra.Infrastructure.Auth.Token;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Factory;
using Orquestra.Infrastructure.Factory.DataBase;
using Orquestra.Infrastructure.Factory.RabbitMQ;
using Orquestra.Infrastructure.Interceptors;
using Orquestra.Infrastructure.Jobs.Companies;
using Orquestra.Infrastructure.Jobs.Schedules;
using Orquestra.Infrastructure.Messaging.Consumers;
using Orquestra.Infrastructure.Messaging.HostedService;
using Orquestra.Infrastructure.Messaging.Publishers;
using Orquestra.Infrastructure.Services.Email;
using Orquestra.Infrastructure.Services.Email.Models;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.Infrastructure.Services.Sms;
using System.Text;
using System.Text.Json;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjectionInfrastructure(this IServiceCollection services, WebApplicationBuilder builder)
    {
        AddServices(services, builder);
        AddAuth(services, builder);
        AddFactory(services, builder);
        AddRabbitMQ(services);
        AddContext(services, builder);
        AddJobs(services);

        return services;
    }

    private static void AddServices(IServiceCollection services, WebApplicationBuilder builder)
    {
        // JWT;
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

        // E-mail;
        services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
        services.AddSingleton<IEmailService>(x =>
        {
            EmailSettings settings = x.GetRequiredService<IOptions<EmailSettings>>().Value;

            if (string.IsNullOrWhiteSpace(settings.SmtpKey))
            {
                throw new ArgumentException("Brevo SMTP key não está configurada.");
            }

            IWebHostEnvironment env = builder.Environment;
            return new EmailService(settings, env);
        });

        // SMS;
        services.AddHttpClient<ISmsService, SmsService>();

        // Env;
        services.AddSingleton<IEnvService>(x =>
        {
            IWebHostEnvironment env = builder.Environment;
            IConfiguration config = builder.Configuration;

            return new EnvService(env, config);
        });
    }

    private static readonly string[] OnAuthenticationFailed = ["Sua sessão expirou. Por favor, realize o login novamente para continuar."];
    private static readonly string[] onChallengeError = ["Acesso negado. É necessário estar autenticado para acessar este recurso."];

    private static void AddAuth(IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).
             AddJwtBearer(x =>
             {
                 x.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
                 x.SaveToken = true;
                 x.IncludeErrorDetails = true;
                 x.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:Secret"] ?? string.Empty)),
                     ValidateIssuer = true,
                     ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                     ValidateAudience = true,
                     ValidAudience = builder.Configuration["JwtSettings:Audience"],
                     ValidateLifetime = true,
                     ClockSkew = TimeSpan.Zero
                 };

                 x.Events = new JwtBearerEvents
                 {
                     // Lidar com requisições em geral, tendo token ou não;
                     OnMessageReceived = context =>
                     {
                         // Se o middleware renovou o token (e agora tem um refresh token), use-o nesta mesma request;
                         if (context.HttpContext.Items.TryGetValue(SystemConsts.Cookies.Refresh, out object? refreshed) && refreshed is string refreshedToken && !string.IsNullOrEmpty(refreshedToken))
                         {
                             context.Token = refreshedToken;
                             return Task.CompletedTask;
                         }

                         // Se não tem um refresh token, ou seja, tem apenas um token original, use-o;
                         if (context.Request.Cookies.ContainsKey(SystemConsts.Cookies.Auth))
                         {
                             context.Token = context.Request.Cookies[SystemConsts.Cookies.Auth];
                             return Task.CompletedTask;
                         }

                         return Task.CompletedTask;
                     },

                     // Lidar com requisições sem autenticação (token ausente);
                     // ou propagar o status de falha definido em OnAuthenticationFailed;
                     OnAuthenticationFailed = context =>
                     {
                         context.NoResult();

                         if (!context.Response.HasStarted)
                         {
                             context.Response.StatusCode = StatusCodes.Status419AuthenticationTimeout;
                         }

                         return Task.CompletedTask;
                     },

                     // Lidar com requisições sem autenticação ou que deram erro;
                     OnChallenge = context =>
                     {
                         int statusCodeJar = context.Response.StatusCode;
                         context.HandleResponse();

                         // Mantém o Status419AuthenticationTimeout se o erro tiver sido configurado no OnAuthenticationFailed,
                         // caso contrário retorna Status401Unauthorized como padrão;
                         int statusCode = statusCodeJar == StatusCodes.Status419AuthenticationTimeout ? StatusCodes.Status419AuthenticationTimeout : StatusCodes.Status401Unauthorized;
                         string[] message = statusCodeJar == StatusCodes.Status419AuthenticationTimeout ? OnAuthenticationFailed : onChallengeError;

                         context.Response.StatusCode = statusCode;
                         context.Response.ContentType = "application/json";

                         string result = JsonSerializer.Serialize(new
                         {
                             Code = statusCode,
                             Date = GetDateDetails(),
                             context.HttpContext.Request.Path,
                             Messages = message,
                             HasError = true
                         });

                         return context.Response.WriteAsync(result);
                     }
                 };
             });
    }

    private static void AddFactory(IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddSingleton<IDataBaseConnection, DataBaseConnection>();

        IConfigurationSection rabbitMQSection = builder.Configuration.GetSection("RabbitMQ");
        string? rabbitMqUrl = rabbitMQSection?.GetValue<string>("Url");
        services.AddSingleton<IRabbitMQConnection>(x => new RabbitMQConnection(rabbitMqUrl: rabbitMqUrl));
    }

    private static void AddRabbitMQ(IServiceCollection services)
    {
        // Publishers;
        services.AddSingleton<IGenericPublisher, GenericPublisher>();

        // Consumers;
        services.AddSingleton(x =>
            new EmailConsumer(
                connection: x.GetRequiredService<IRabbitMQConnection>(),
                emailService: x.GetRequiredService<IEmailService>(),
                queueName: SystemConsts.RabbitMQ.EmailQueue
            )
        );

        // Hosted services;
        services.AddHostedService<EmailConsumerHostedService>();
    }

    private static void AddContext(IServiceCollection services, WebApplicationBuilder builder)
    {
        string con = new DataBaseConnection(builder.Configuration).GetConnectionString();

        services.AddDbContextPool<Context>((serviceProvider, options) =>
        {
            ILogger<SlowQueryDebugInterceptor> logger = serviceProvider.GetRequiredService<ILogger<SlowQueryDebugInterceptor>>();
            IWebHostEnvironment env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
            SlowQueryDebugInterceptor interceptor = new(logger, env);

            options.UseNpgsql(con).AddInterceptors(interceptor);
        });
    }

    private static void AddJobs(IServiceCollection services)
    {
        services.AddHostedService<CompanyPlanJob>();
        services.AddHostedService<ScheduleStatusJob>();
    }
}