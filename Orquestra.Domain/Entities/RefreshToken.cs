using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Orquestra.Domain.Entities;

public sealed class RefreshToken
{
    [Key]
    public Guid RefreshTokenId { get; set; }

    public string Token { get; set; } = string.Empty;

    public Guid UserId { get; set; }
    [JsonIgnore]
    public User? User { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ExpiredDate { get; set; }

    public DateTime? RevokedDate { get; set; }
}