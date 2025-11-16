using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orquestra.Domain.Entities;

public sealed class QuoteItem
{
    [Key]
    public Guid QuoteItemId { get; set; }

    public Guid QuoteId { get; set; }
    [ForeignKey(nameof(QuoteId))]
    public Quote? Quote { get; set; }

    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public int Quantity { get; set; } = 1;

    [Precision(18, 2)]
    public decimal UnitPrice { get; set; }

    [Precision(18, 2)]
    public decimal TotalPrice { get; set; }
}