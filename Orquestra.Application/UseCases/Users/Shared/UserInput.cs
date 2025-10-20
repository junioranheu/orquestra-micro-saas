using Orquestra.Domain.Enums;
using System.Text.Json.Serialization;

namespace Orquestra.Application.UseCases.Users.Shared;

public sealed class UserInput
{
    [JsonIgnore]
    public Guid? UserId { get; set; } = Guid.Empty;

    public string? FullName { get; set; } = string.Empty;

    public string? Email { get; set; } = string.Empty;

    public string? Password { get; set; } = string.Empty;

    public RecoverPasswordQuestionEnum RecoverPasswordQuestion { get; set; }

    public string RecoverPasswordAnswer { get; set; } = string.Empty;

    public string? InviteToken { get; set; } = string.Empty;
}