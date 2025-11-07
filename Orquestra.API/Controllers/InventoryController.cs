using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Inventories.Create;
using Orquestra.Application.UseCases.Inventories.Delete;
using Orquestra.Application.UseCases.Inventories.GetAllByCompanyId;
using Orquestra.Application.UseCases.Inventories.Shared;
using Orquestra.Application.UseCases.Inventories.Update;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController(
        IGetAllInventoryByCompanyId getInventoryByCompanyId,
        ICreateInventory create,
        IUpdateInventory update,
        IDeleteInventory delete
    ) : BaseController<InventoryController>
{
    private readonly IGetAllInventoryByCompanyId _getInventoryByCompanyId = getInventoryByCompanyId;
    private readonly ICreateInventory _create = create;
    private readonly IUpdateInventory _update = update;
    private readonly IDeleteInventory _delete = delete;

    [AuthorizeFilter]
    [HttpPost]
    public async Task<ActionResult> Create(InventoryInput input)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _create.Execute(userIdAuth, input);

        return Ok(true);
    }

    [AuthorizeFilter]
    [HttpPut]
    public async Task<ActionResult> Update(InventoryInput input)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _update.Execute(userIdAuth, input);

        return Ok(true);
    }

    [AuthorizeFilter]
    [HttpPut("Disable")]
    public async Task<ActionResult> Disable(InventoryInput input)
    {
        if (input.InventoryId == Guid.Empty || input.InventoryId is null)
        {
            throw new ArgumentException($"O parâmetro {nameof(input.InventoryId)} está vazio.");
        }

        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _delete.Execute(userIdAuth, input.InventoryId.GetValueOrDefault());

        return Ok(true);
    }

    [AuthorizeFilter]
    [HttpGet("GetAllByCompanyId")]
    public async Task<ActionResult> GetAllByCompanyId([FromQuery] PaginationInput paginationInput, Guid? companyId, InventoryInput input)
    {
        if (companyId is null || companyId == Guid.Empty)
        {
            return NoContent();
        }

        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        (IEnumerable<Inventory> output, int count) = await _getInventoryByCompanyId.Execute(paginationInput, userIdAuth, companyId.GetValueOrDefault(), input);

        return Ok(new { output, count });
    }
}