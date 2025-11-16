using Microsoft.EntityFrameworkCore;
using Orquestra.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orquestra.Domain.Entities;

public sealed class Quote : Audit
{
    [Key]
    public Guid QuoteId { get; set; }

    public Guid CompanyId { get; set; }
    [ForeignKey(nameof(CompanyId))]
    public Company? Company { get; set; }

    public Guid ClientId { get; set; }
    [ForeignKey(nameof(ClientId))]
    public Client? Client { get; set; }

    [MaxLength(300)]
    public string? Title { get; set; }

    [MaxLength(500)]
    public string? Observation { get; set; }

    public DateTime? ValidUntil { get; set; }

    [MaxLength(50)]
    public QuoteStatusEnum QuoteStatus { get; set; } = QuoteStatusEnum.Draft;

    [Precision(18, 2)]
    public decimal TotalAmount { get; set; }

    public List<QuoteItem> Items { get; set; } = [];
}