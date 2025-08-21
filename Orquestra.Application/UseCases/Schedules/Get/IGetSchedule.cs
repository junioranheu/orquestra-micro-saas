using Orquestra.Application.UseCases.Schedules.Shared;

namespace Orquestra.Application.UseCases.Schedules.Get;

public interface IGetSchedule
{
    Task<ScheduleOutput?> Execute(Guid userIdAuth, Guid scheduleId);
}