using Orquestra.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Domain.Entities;

public sealed class Log
{
    [Key]
    public Guid LogId { get; set; }

    public required LogTypeEnum LogType { get; set; }

    public string? RequestType { get; set; } = string.Empty;

    public string? Endpoint { get; set; } = string.Empty;

    public string? Parameters { get; set; } = string.Empty;

    public string? Exception { get; set; } = string.Empty;

    public string? Description { get; set; } = string.Empty;

    public int Status { get; set; }

    public Guid? UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User? User { get; init; }

    public DateTime CreatedDate { get; set; } = GetDate();
}