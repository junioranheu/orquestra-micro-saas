using Orquestra.Application.UseCases.Schedules.Shared;

namespace Orquestra.Application.UseCases.Schedules.Get;

public interface IGetSchedule
{
    Task<List<ScheduleOutput>?> Execute();
    Task<ScheduleOutput?> Execute(Guid scheduleId);
}