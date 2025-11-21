using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Schedules.Create;
using Orquestra.Application.UseCases.Schedules.Delete;
using Orquestra.Application.UseCases.Schedules.Get;
using Orquestra.Application.UseCases.Schedules.GetAllByClientId;
using Orquestra.Application.UseCases.Schedules.GetAllByCompanyId;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Application.UseCases.Schedules.Update;
using Orquestra.Domain.Enums;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScheduleController(
        IGetSchedule get,
        IGetAllScheduleByCompanyId getScheduleByCompanyId,
        IGetAllScheduleByClientId getScheduleByClientId,
        ICreateSchedule create,
        IUpdateSchedule update,
        IDeleteSchedule delete
    ) : BaseController<ScheduleController>
{
    private readonly IGetSchedule _get = get;
    private readonly IGetAllScheduleByCompanyId _getScheduleByCompanyId = getScheduleByCompanyId;
    private readonly IGetAllScheduleByClientId _getScheduleByClientId = getScheduleByClientId;
    private readonly ICreateSchedule _create = create;
    private readonly IUpdateSchedule _update = update;
    private readonly IDeleteSchedule _delete = delete;

    [AuthorizeFilter(modules: [ModuleEnum.Scheduling])]
    [HttpPost]
    public async Task<ActionResult> Create(ScheduleInput input)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        ScheduleOutput output = await _create.Execute(userIdAuth, input);

        return Ok(output);
    }

    [AuthorizeFilter(modules: [ModuleEnum.Scheduling])]
    [HttpPut]
    public async Task<ActionResult> Update(ScheduleInput input)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        ScheduleOutput output = await _update.Execute(userIdAuth, input);

        return Ok(output);
    }

    [AuthorizeFilter(modules: [ModuleEnum.Scheduling])]
    [HttpPut("Disable")]
    public async Task<ActionResult> Disable(ScheduleInput input)
    {
        if (input.ScheduleId == Guid.Empty || input.ScheduleId is null)
        {
            throw new ArgumentException($"O parâmetro {nameof(input.ScheduleId)} está vazio.");
        }

        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _delete.Execute(userIdAuth, input.ScheduleId.GetValueOrDefault());

        return Ok(true);
    }

    [AuthorizeFilter(modules: [ModuleEnum.Scheduling])]
    [HttpGet]
    public async Task<ActionResult> Get(Guid scheduleId)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        ScheduleOutput? output = await _get.Execute(userIdAuth, scheduleId);

        return Ok(output);
    }

    [AuthorizeFilter(modules: [ModuleEnum.Scheduling])]
    [HttpGet("GetAllByCompanyId")]
    public async Task<ActionResult> GetAllByCompanyId(Guid companyId, int? year = null, int? month = null, bool? getOnlyNearbyDates = false)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        List<ScheduleOutput>? output = await _getScheduleByCompanyId.Execute(userIdAuth, companyId, year, month, getOnlyNearbyDates);

        return Ok(output);
    }

    [AuthorizeFilter(modules: [ModuleEnum.Scheduling])]
    [HttpGet("GetAllByClientId")]
    public async Task<ActionResult> GetScheduleByClientId(Guid companyId, Guid clientId)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        List<ScheduleOutput>? output = await _getScheduleByClientId.Execute(userIdAuth, companyId, clientId);

        return Ok(output);
    }
}