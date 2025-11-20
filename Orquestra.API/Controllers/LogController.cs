using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Logs.Get;
using Orquestra.Application.UseCases.Logs.GetNotification;
using Orquestra.Application.UseCases.Logs.Shared;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogController(IGetLog get, IGetNotificationLog getNotificationLog) : BaseController<LogController>
{
    private readonly IGetLog _get = get;
    private readonly IGetNotificationLog _getNotificationLog = getNotificationLog;

    [AuthorizeFilter(roles: [UserRoleEnum.Administrator, UserRoleEnum.Maintainer])]
    [HttpGet]
    public async Task<ActionResult> Get([FromQuery] PaginationInput paginationInput, [FromQuery] Guid? userId)
    {
        (IEnumerable<Log> output, int count) = await _get.Execute(paginationInput, userId);

        return Ok(new { output, count });
    }

    [AuthorizeFilter]
    [HttpGet("GetNotification")]
    public async Task<ActionResult> GetNotification([FromQuery] PaginationInput paginationInput, bool? isDashboard = false)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        (List<LogNotificationOutput> output, int count) = await _getNotificationLog.Execute(paginationInput, userIdAuth, isDashboard.GetValueOrDefault());

        return Ok(new { output, count });
    }
}