using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.ServiceOrders.Create;
using Orquestra.Application.UseCases.ServiceOrders.Delete;
using Orquestra.Application.UseCases.ServiceOrders.GetAllByCompanyId;
using Orquestra.Application.UseCases.ServiceOrders.Shared;
using Orquestra.Application.UseCases.ServiceOrders.Update;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Enums;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceOrderController(
        IGetAllServiceOrderByCompanyId getServiceOrderByCompanyId,
        ICreateServiceOrder create,
        IUpdateServiceOrder update,
        IDeleteServiceOrder delete
    ) : BaseController<ServiceOrderController>
{
    private readonly IGetAllServiceOrderByCompanyId _getServiceOrderByCompanyId = getServiceOrderByCompanyId;
    private readonly ICreateServiceOrder _create = create;
    private readonly IUpdateServiceOrder _update = update;
    private readonly IDeleteServiceOrder _delete = delete;

    [AuthorizeFilter(modules: [ModuleEnum.ServiceOrder])]
    [HttpPost]
    public async Task<ActionResult> Create(ServiceOrderInput input)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _create.Execute(userIdAuth, input);

        return Ok(true);
    }

    [AuthorizeFilter(modules: [ModuleEnum.ServiceOrder])]
    [HttpPut]
    public async Task<ActionResult> Update(ServiceOrderInput input)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _update.Execute(userIdAuth, input);

        return Ok(true);
    }

    [AuthorizeFilter(modules: [ModuleEnum.ServiceOrder])]
    [HttpPut("Disable")]
    public async Task<ActionResult> Disable(Guid serviceOrderId)
    {
        if (serviceOrderId == Guid.Empty)
        {
            throw new ArgumentException($"O parâmetro {nameof(serviceOrderId)} está vazio.");
        }

        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _delete.Execute(userIdAuth, serviceOrderId);

        return Ok(true);
    }

    [AuthorizeFilter(modules: [ModuleEnum.ServiceOrder])]
    [HttpGet("GetAllByCompanyId")]
    public async Task<ActionResult> GetAllByCompanyId([FromQuery] PaginationInput paginationInput, [FromQuery] ServiceOrderInput input)
    {
        if (input.CompanyId == Guid.Empty || input.CompanyId is null)
        {
            return NoContent();
        }

        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        (IEnumerable<ServiceOrderOutput> output, int count) = await _getServiceOrderByCompanyId.Execute(paginationInput, input, userIdAuth, companyId: input.CompanyId.GetValueOrDefault());

        return Ok(new { output, count });
    }
}