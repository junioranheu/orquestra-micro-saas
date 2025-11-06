using Orquestra.Application.UseCases.Schedules.Shared;

namespace Orquestra.Application.UseCases.Schedules.GetAllByClientId;

public interface IGetAllScheduleByClientId
{
    Task<List<ScheduleOutput>?> Execute(Guid userIdAuth, Guid companyId, Guid clientId);
}