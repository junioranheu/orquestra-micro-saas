namespace Orquestra.Application.UseCases.Sales.Shared;

public sealed class SalesChartOutput
{
    public required string Type { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required decimal Value { get; set; }
    public required DateTime? Date { get; set; }
}