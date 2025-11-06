using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orquestra.Domain.Entities;

public sealed class Inventory : Audit
{
    [Key]
    public Guid InventoryId { get; set; }

    public Guid CompanyId { get; set; }
    [ForeignKey(nameof(CompanyId))]
    public Company? Company { get; set; }

    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Description { get; set; }

    public int Quantity { get; set; }
    public decimal? UnitPrice { get; set; }

    public byte[]? Image { get; set; }
    public string? ImageContentType { get; set; }

    [NotMapped]
    public decimal? TotalValue => UnitPrice.HasValue ? Quantity * UnitPrice.Value : null;
}