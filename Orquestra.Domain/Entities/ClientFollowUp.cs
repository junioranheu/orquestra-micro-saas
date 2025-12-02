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

    public Guid CompanyId { get; set; }
    [ForeignKey(nameof(CompanyId))]
    public Company? Company { get; set; }

    public Guid? ScheduleId { get; set; }
    [ForeignKey(nameof(ScheduleId))]
    public Schedule? Schedule { get; set; }

    [MaxLength(1024)]
    public string? Observation { get; set; }

    public ClientFollowUpStatusEnum ClientFollowUpStatus { get; set; } = ClientFollowUpStatusEnum.InProgress;

    public List<byte[]>? Images { get; set; } = [];

    public List<string>? ImagesContentType { get; set; } = [];
}