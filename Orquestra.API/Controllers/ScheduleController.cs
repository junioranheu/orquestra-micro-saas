using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Schedules.Create;
using Orquestra.Application.UseCases.Schedules.Get;
using Orquestra.Application.UseCases.Schedules.GetAllByCompanyId;
using Orquestra.Application.UseCases.Schedules.Shared;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScheduleController(
        IGetSchedule get,
        IGetScheduleByCompanyId getScheduleByCompanyId,
        ICreateSchedule create
    ) : BaseController<ScheduleController>
{
    private readonly IGetSchedule _get = get;
    private readonly IGetScheduleByCompanyId _getScheduleByCompanyId = getScheduleByCompanyId;
    private readonly ICreateSchedule _create = create;

    [AuthorizeFilter]
    [HttpPost]
    public async Task<ActionResult> Create([FromForm] ScheduleInput input)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        ScheduleOutput output = await _create.Execute(userIdAuth, input);

        return Ok(output);
    }

    [AuthorizeFilter]
    [HttpGet]
    public async Task<ActionResult> Get(Guid scheduleId)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        ScheduleOutput? output = await _get.Execute(userIdAuth, scheduleId);

        return Ok(output);
    }

    [AuthorizeFilter]
    [HttpGet("GetAllByCompanyId")]
    public async Task<ActionResult> GetAllByCompanyId(Guid companyId, int? year = null, int? month = null)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        List<ScheduleOutput>? output = await _getScheduleByCompanyId.Execute(userIdAuth, companyId, year, month);

        return Ok(output);
    }
}