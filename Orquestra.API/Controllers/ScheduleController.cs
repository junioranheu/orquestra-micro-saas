using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Schedules.Get;
using Orquestra.Application.UseCases.Schedules.Shared;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScheduleController(IGetSchedule get) : BaseController<ScheduleController>
{
    private readonly IGetSchedule _get = get;

    [AuthorizeFilter]
    [HttpPost]
    public async Task<ActionResult<ScheduleOutput>> Create([FromForm] ScheduleInput input)
    {
        Guid userId = GetUserId(throwExceptionIfNotAuth: true);

        ScheduleOutput output = await _create.Execute(userId, input);
        return output;
    }

    [AuthorizeFilter]
    [HttpGet]
    public async Task<ActionResult<ScheduleOutput?>> Get(Guid scheduleId)
    {
        ScheduleOutput? output = await _get.Execute(scheduleId);
        return output;
    }

    [AuthorizeFilter]
    [HttpGet("GetAll")]
    public async Task<ActionResult<List<ScheduleOutput>?>> GetAll()
    {
        List<ScheduleOutput>? output = await _get.Execute();
        return output;
    }
}