using Orquestra.Application.UseCases.Sales.Shared;

namespace Orquestra.Application.UseCases.Sales.GetChart;

public interface IGetChartSales
{
    Task<List<SalesChartOutput>> Execute(Guid userIdAuth, Guid companyId);
}