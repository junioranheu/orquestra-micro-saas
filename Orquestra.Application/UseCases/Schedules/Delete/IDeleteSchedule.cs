namespace Orquestra.Application.UseCases.Schedules.Delete;

public interface IDeleteSchedule
{
    Task Execute(Guid userIdAuth, Guid scheduleId);
}