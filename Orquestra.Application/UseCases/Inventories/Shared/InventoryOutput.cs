namespace Orquestra.Application.UseCases.Inventories.Shared;

public sealed class InventoryOutput
{
    public Guid? InventoryId { get; set; }
    public Guid? CompanyId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? TotalValue { get; set; }
    public string? ImageBase64 { get; set; } // Base64;
    public string? ImageContentType { get; set; }
}