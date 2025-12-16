using Orquestra.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orquestra.Domain.Entities;

public sealed class ServiceOrder : Audit
{
    [Key]
    public Guid ServiceOrderId { get; set; }

    public Guid CompanyId { get; set; }
    [ForeignKey(nameof(CompanyId))]
    public Company? Company { get; set; }

    public Guid? QuoteId { get; set; } // Nullable;
    [ForeignKey(nameof(QuoteId))]
    public Quote? Quote { get; set; }

    public Guid ClientId { get; set; }
    [ForeignKey(nameof(ClientId))]
    public Client? Client { get; set; }

    [MaxLength(120)]
    public string? Title { get; set; }

    [MaxLength(255)]
    public string? Observation { get; set; }

    public DateTime? ExecutionDate { get; set; }

    public ServiceOrderStatusEnum ServiceOrderStatus { get; set; } = ServiceOrderStatusEnum.Pending;
}