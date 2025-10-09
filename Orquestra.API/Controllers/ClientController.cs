using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Clients.Create;
using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Clients.GetAllByCompanyId;
using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.Shared;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientController(
        IGetClient get,
        IGetClientByCompanyId getClientByCompanyId,
        ICreateClient create
    ) : BaseController<ClientController>
{
    private readonly IGetClient _get = get;
    private readonly IGetClientByCompanyId _getClientByCompanyId = getClientByCompanyId;
    private readonly ICreateClient _create = create;

    [AuthorizeFilter]
    [HttpPost]
    public async Task<ActionResult> Create([FromForm] ClientInput input)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _create.Execute(userIdAuth, input);

        return NoContent();
    }

    [AuthorizeFilter]
    [HttpGet]
    public async Task<ActionResult> Get(Guid clientId)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        ClientOutput? output = await _get.Execute(userIdAuth, clientId);

        return Ok(output);
    }

    [AuthorizeFilter]
    [HttpGet("GetAllByCompanyId")]
    public async Task<ActionResult> GetAllByCompanyId([FromQuery] PaginationInput paginationInput, Guid companyId)
    {
        if (companyId == Guid.Empty)
        {
            return NoContent();
        }

        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        (IEnumerable<ClientOutput> output, int count) = await _getClientByCompanyId.Execute(paginationInput, userIdAuth, companyId);

        return Ok(new { output, count });
    }
}