using Orquestra.Application.UseCases.Schedules.Shared;

namespace Orquestra.Application.UseCases.Schedules.Create;

public interface ICreateSchedule
{
    Task<ScheduleOutput> Execute(Guid userId, ScheduleInput input);
}