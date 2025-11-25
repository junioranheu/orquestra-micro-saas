using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orquestra.Domain.Entities;

public sealed class QuoteItem : Audit
{
    [Key]
    public Guid QuoteItemId { get; set; }

    public Guid QuoteId { get; set; }
    [ForeignKey(nameof(QuoteId))]
    public Quote? Quote { get; set; }

    [MaxLength(120)]
    public string Title { get; set; } = string.Empty;

    public int Quantity { get; set; } = 1;

    [Precision(10, 2)]
    public decimal UnitPrice { get; set; }

    [Precision(10, 2)]
    public decimal TotalPrice { get; set; }

    [NotMapped]
    public decimal? TotalValue => Quantity * UnitPrice;
}