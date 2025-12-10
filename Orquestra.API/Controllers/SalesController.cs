using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Sales.GetChart;
using Orquestra.Application.UseCases.Sales.Shared;
using Orquestra.Domain.Enums;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController(IGetChartSales getChart) : BaseController<SalesController>
{
    private readonly IGetChartSales _getChart = getChart;

    [AuthorizeFilter(modules: [ModuleEnum.Sales])]
    [HttpGet("GetChart")]
    public async Task<ActionResult> GetChart([FromQuery] Guid companyId)
    {
        if (companyId == Guid.Empty)
        {
            return NoContent();
        }

        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        SalesOutput output = await _getChart.Execute(userIdAuth, companyId);

        return Ok(output);
    }
}