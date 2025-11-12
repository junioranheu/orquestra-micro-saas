using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.ClientsFollowUps.Create;
using Orquestra.Application.UseCases.ClientsFollowUps.Delete;
using Orquestra.Application.UseCases.ClientsFollowUps.Get;
using Orquestra.Application.UseCases.ClientsFollowUps.Shared;
using Orquestra.Application.UseCases.ClientsFollowUps.Update;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientFollowUpController(
        IGetClientFollowUp get,
        ICreateClientFollowUp create,
        IUpdateClientFollowUp update,
        IDeleteClientFollowUp delete
    ) : BaseController<ClientFollowUpController>
{
    private readonly IGetClientFollowUp _get = get;
    private readonly ICreateClientFollowUp _create = create;
    private readonly IUpdateClientFollowUp _update = update;
    private readonly IDeleteClientFollowUp _delete = delete;

    [AuthorizeFilter]
    [HttpPost]
    public async Task<ActionResult> Create(ClientFollowUpInput input)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _create.Execute(userIdAuth, input);

        return Ok(true);
    }

    [AuthorizeFilter]
    [HttpPut]
    public async Task<ActionResult> Update(ClientFollowUpInput input)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _update.Execute(userIdAuth, input);

        return Ok(true);
    }

    [AuthorizeFilter]
    [HttpPut("Disable")]
    public async Task<ActionResult> Disable(ClientFollowUpInput input)
    {
        if (input.ClientFollowUpId == Guid.Empty || input.ClientFollowUpId is null)
        {
            throw new ArgumentException($"O parâmetro {nameof(input.ClientFollowUpId)} está vazio.");
        }

        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _delete.Execute(userIdAuth, input.ClientFollowUpId.GetValueOrDefault());

        return Ok(true);
    }

    [AuthorizeFilter]
    [HttpGet("GetAllByCompanyId")]
    public async Task<ActionResult> GetAllByCompanyId(ClientFollowUpInput input)
    {
        if (input is null || input.ClientId is null || input.ClientId == Guid.Empty)
        {
            return NoContent();
        }

        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        (IEnumerable<ClientFollowUpOutput> output, int count) = await _get.Execute(userIdAuth, input);

        return Ok(new { output, count });
    }
}