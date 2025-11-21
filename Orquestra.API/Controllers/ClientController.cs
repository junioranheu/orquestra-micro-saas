using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Clients.Create;
using Orquestra.Application.UseCases.Clients.Delete;
using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Clients.GetAllByCompanyId;
using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.Clients.Update;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Enums;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientController(
        IGetClient get,
        IGetAllClientByCompanyId getClientByCompanyId,
        ICreateClient create,
        IUpdateClient update,
        IDeleteClient delete
    ) : BaseController<ClientController>
{
    private readonly IGetClient _get = get;
    private readonly IGetAllClientByCompanyId _getClientByCompanyId = getClientByCompanyId;
    private readonly ICreateClient _create = create;
    private readonly IUpdateClient _update = update;
    private readonly IDeleteClient _delete = delete;

    [AuthorizeFilter(modules: [ModuleEnum.Client])]
    [HttpPost]
    public async Task<ActionResult> Create(ClientInput input)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _create.Execute(userIdAuth, input);

        return Ok(true);
    } 

    [AuthorizeFilter(modules: [ModuleEnum.Client])]
    [HttpPut]
    public async Task<ActionResult> Update(ClientInput input)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _update.Execute(userIdAuth, input);

        return Ok(true);
    }

    [AuthorizeFilter(modules: [ModuleEnum.Client])]
    [HttpPut("Disable")]
    public async Task<ActionResult> Disable(ClientInput input)
    {
        if (input.ClientId == Guid.Empty || input.ClientId is null)
        {
            throw new ArgumentException($"O parâmetro {nameof(input.ClientId)} está vazio.");
        }

        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _delete.Execute(userIdAuth, input.ClientId.GetValueOrDefault());

        return Ok(true);
    }

    [AuthorizeFilter(modules: [ModuleEnum.Client])]
    [HttpGet]
    public async Task<ActionResult> Get(Guid clientId)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        ClientOutput? output = await _get.Execute(userIdAuth, clientId);

        return Ok(output);
    }

    [AuthorizeFilter(modules: [ModuleEnum.Client])]
    [HttpGet("GetAllByCompanyId")]
    public async Task<ActionResult> GetAllByCompanyId([FromQuery] PaginationInput paginationInput, [FromQuery] ClientInput input)
    {
        if (input.CompanyId == Guid.Empty || input.CompanyId is null)
        {
            return NoContent();
        }

        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        (IEnumerable<ClientOutput> output, int count) = await _getClientByCompanyId.Execute(paginationInput, input, userIdAuth, companyId: input.CompanyId.GetValueOrDefault());

        return Ok(new { output, count });
    }
}