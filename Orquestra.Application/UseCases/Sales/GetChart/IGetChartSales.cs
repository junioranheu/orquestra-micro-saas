using Orquestra.Application.UseCases.Sales.Shared;
using Orquestra.Application.UseCases.Shared;

namespace Orquestra.Application.UseCases.Sales.GetChart;

public interface IGetChartSales
{
    Task<SalesOutput> Execute(PaginationInput pagination, Guid userIdAuth, Guid companyId);
}