namespace Orquestra.Application.UseCases.Sales.Shared;

public sealed class SalesOutput
{
    public required List<SalesTableOutput> Table { get; set; } = [];
    public required int TableTotalCount { get; set; }
    public required List<SalesChartOutput> Chart { get; set; } = [];
    public required decimal CashInflow { get; set; }
    public required decimal CashOutflow { get; set; }
    public required decimal FinalBalance { get; set; }
}

public sealed class SalesTableOutput
{
    public required Guid Id { get; set; }
    public required string Type { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required decimal Value { get; set; }
    public required DateTime? Date { get; set; }
}

public sealed class SalesChartOutput
{
    public required string Type { get; set; } = null!;
    public required string Color { get; set; } = null!;
    public List<SalesChartItemOutput> Items { get; set; } = [];
}

public sealed class SalesChartItemOutput
{
    public string DateTime { get; set; } = null!;
    public decimal Value { get; set; }
}