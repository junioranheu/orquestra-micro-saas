using Orquestra.Application.UseCases.Sales.Shared;

namespace Orquestra.Application.UseCases.Sales.GetChart;

public interface IGetChartSales
{
    Task<SalesOutput> Execute(Guid userIdAuth, Guid companyId);
}