using Orquestra.Application.UseCases.Schedules.Shared;

namespace Orquestra.Application.UseCases.Schedules.GetAllByClientId;

public interface IGetScheduleByClientId
{
    Task<List<ScheduleOutput>?> Execute(Guid userIdAuth, Guid companyId, Guid clientId);
}