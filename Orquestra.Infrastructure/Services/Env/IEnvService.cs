using Orquestra.Infrastructure.Services.Env.Models;

namespace Orquestra.Infrastructure.Services.Env;

public interface IEnvService
{
    bool IsDevelopment();
    bool IsProduction();
    EnvOutput GetUrls();
}