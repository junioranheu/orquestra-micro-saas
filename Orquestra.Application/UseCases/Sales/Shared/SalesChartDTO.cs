namespace Orquestra.Application.UseCases.Sales.Shared;

public sealed class SalesChartDTO
{
    public required string Type { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required decimal Value { get; set; }
    public required DateTime? Date { get; set; }
}

public sealed class SalesChartOutput
{
    public string Type { get; set; } = null!;
    public string Color { get; set; } = null!;
    public List<SalesChartItemOutput> Items { get; set; } = [];
}

public sealed class SalesChartItemOutput
{
    public string DateTime { get; set; } = null!;
    public decimal Value { get; set; }
}