using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Logs.Get;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogController(IGetLog get) : Controller
{
    private readonly IGetLog _get = get;

    [AuthorizeFilter(UserRoleEnum.Admin, UserRoleEnum.Maintainer)]
    [HttpGet]
    public async Task<ActionResult> Create([FromQuery] PaginationInput paginationInput, [FromQuery] Guid? userId)
    {
        (IEnumerable<Log> linq, int count) = await _get.Execute(paginationInput, userId);
        return Ok(new { linq, count });
    }
}