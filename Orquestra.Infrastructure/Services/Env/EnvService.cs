using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Orquestra.Infrastructure.Services.Env.Models;

namespace Orquestra.Infrastructure.Services.Env;

public class EnvService(IWebHostEnvironment env, IConfiguration configuration) : IEnvService
{
    private readonly bool _isDevelopment = env.IsDevelopment();
    private readonly IConfiguration _configuration = configuration;

    public bool IsDevelopment()
    {
        return _isDevelopment;
    }

    public EnvOutput GetUrls()
    {
        string sectionName = _isDevelopment ? "Development" : "Production";
        IConfigurationSection urlsSection = _configuration.GetSection($"Urls:{sectionName}");

        return new EnvOutput
        {
            UrlBackend = urlsSection["Backend"] ?? throw new Exception("Erro crítico interno: URLs back-end não definidas."),
            UrlFrontend = urlsSection["Frontend"] ?? throw new Exception("Erro crítico interno: URLs front-end não definidas.")
        };
    }
}