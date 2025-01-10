using Orquestra.Domain.Entities;

namespace Orquestra.Application.UseCases.Users.Shared;

public sealed class UserOutput
{
    public Guid UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool IsVerified { get; set; } = false;

    public bool Status { get; set; }

    public IEnumerable<UserRole>? UserRoles { get; init; }

    // Extras;
    public string Token { get; set; } = string.Empty;
}