namespace Orquestra.Application.UseCases.Auth.Shared;

public sealed class AuthInput
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}