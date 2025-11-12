using Orquestra.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orquestra.Domain.Entities;

public sealed class ClientFollowUp : Audit
{
    [Key]
    public Guid ClientFollowUpId { get; set; }

    public Guid ClientId { get; set; }
    [ForeignKey(nameof(ClientId))]
    public Client? Client { get; set; }

    [MaxLength(1024)]
    public string? Observation { get; set; }

    public ClientFollowUpStatus ClientFollowUpStatus { get; set; } = ClientFollowUpStatus.Pending;

    public List<byte[]>? Images { get; set; } = [];

    public List<string>? ImagesContentType { get; set; } = [];
}