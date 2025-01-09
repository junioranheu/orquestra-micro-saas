using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Users.Shared;

public sealed class UserInput
{
    public string FullName { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public UserRoleEnum UserRole { get; set; }
}