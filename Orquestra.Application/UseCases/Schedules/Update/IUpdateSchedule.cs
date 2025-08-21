using Orquestra.Application.UseCases.Schedules.Shared;

namespace Orquestra.Application.UseCases.Schedules.Update;

public interface IUpdateSchedule
{
    Task<ScheduleOutput> Execute(Guid userIdAuth, ScheduleInput input);
}