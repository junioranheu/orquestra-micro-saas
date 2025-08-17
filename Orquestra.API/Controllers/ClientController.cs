using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Clients.Create;
using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Clients.Shared;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientController(IGetClient get, ICreateClient create) : BaseController<ClientController>
{
    private readonly IGetClient _get = get;
    private readonly ICreateClient _create = create;

    [AuthorizeFilter]
    [HttpPost]
    public async Task<ActionResult<ClientOutput>> Create([FromForm] ClientInput input)
    {
        Guid userId = GetUserId(throwExceptionIfNotAuth: true);
        ClientOutput output = await _create.Execute(userId, input);

        return Ok(output);
    }

    [AuthorizeFilter]
    [HttpGet]
    public async Task<ActionResult<ClientOutput?>> Get(Guid clientId)
    {
        ClientOutput? output = await _get.Execute(clientId);
        return Ok(output);
    }

    [AuthorizeFilter]
    [HttpGet("GetAll")]
    public async Task<ActionResult<List<ClientOutput>?>> GetAll(Guid companyId)
    {
        List<ClientOutput>? output = await _get.GetAll(companyId);
        return Ok(output);
    }
}