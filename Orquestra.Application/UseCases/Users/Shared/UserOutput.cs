using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Users.Shared;

public sealed class UserOutput
{
    public Guid UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public UserRoleEnum Role { get; set; }

    public bool Status { get; set; }

    public DateTime CreatedDate { get; set; }

    // Extra;
    public DateTimeOffset? TokenExpirationDate { get; set; }
}