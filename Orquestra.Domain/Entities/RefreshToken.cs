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

    public DateTime? ExpiredDate { get; set; }

    public DateTime CreatedDate { get; set; } = GetDate();

    public DateTime? RevokedDate { get; set; }

    public bool Status { get; set; } = true;
}