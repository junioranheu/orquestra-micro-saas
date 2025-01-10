using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Domain.Entities;

public sealed class RefreshToken
{
    [Key]
    public Guid RefreshTokenId { get; set; }

    public string Token { get; set; } = string.Empty;

    public Guid UserId { get; set; }
    [JsonIgnore]
    public User? Users { get; set; }

    public DateTime? Expires { get; set; }

    public DateTime Created { get; set; } = GetDate();

    public DateTime? Revoked { get; set; }

    public bool Status { get; set; } = true;
}