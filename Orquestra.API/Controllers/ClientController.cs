using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Clients.Create;
using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Clients.GetByCompanyId;
using Orquestra.Application.UseCases.Clients.Shared;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientController(IGetClient get, IGetClientByCompanyId getClientByCompanyId, ICreateClient create) :
    BaseController<ClientController>
{
    private readonly IGetClient _get = get;
    private readonly IGetClientByCompanyId _getClientByCompanyId = getClientByCompanyId;
    private readonly ICreateClient _create = create;

    [AuthorizeFilter]
    [HttpPost]
    public async Task<ActionResult<ClientOutput>> Create([FromForm] ClientInput input)
    {
        Guid userId = GetUserId(throwExceptionIfNotAuth: true);
        await _create.Execute(userId, input);

        return Ok();
    }

    [AuthorizeFilter]
    [HttpGet]
    public async Task<ActionResult<ClientOutput?>> Get(Guid clientId)
    {
        ClientOutput? output = await _get.Execute(clientId);
        return Ok(output);
    }

    [AuthorizeFilter]
    [HttpGet("GetAllByCompanyId")]
    public async Task<ActionResult<List<ClientOutput>?>> GetAllByCompanyId(Guid companyId)
    {
        List<ClientOutput>? output = await _getClientByCompanyId.Execute(companyId);
        return Ok(output);
    }
}