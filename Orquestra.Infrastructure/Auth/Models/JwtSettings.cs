namespace Orquestra.Infrastructure.Auth.Models;

public sealed class JwtSettings
{
    public string? Secret { get; init; } = null;
    public int TokenExpiryMinutes { get; init; }
    public int RefreshTokenExpiryMinutes { get; init; }
    public string? Issuer { get; init; } = null;
    public string? Audience { get; init; } = null;
}