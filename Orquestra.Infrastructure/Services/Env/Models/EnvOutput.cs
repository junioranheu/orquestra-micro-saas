namespace Orquestra.Infrastructure.Services.Env.Models;

public sealed class EnvOutput
{
    public required string UrlBackend { get; set; }
    public required string UrlFrontend { get; set; }
}