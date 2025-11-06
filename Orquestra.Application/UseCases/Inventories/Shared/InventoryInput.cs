using Microsoft.AspNetCore.Http;

namespace Orquestra.Application.UseCases.Inventories.Shared;

public sealed class InventoryInput
{
    public Guid? InventoryId { get; set; }
    public Guid? CompanyId { get; set; }
    public string? Name { get; set; } 
    public string? Description { get; set; }
    public int? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public IFormFile? ImageFormFile { get; set; }
}