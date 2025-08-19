using Orquestra.Application.UseCases.Schedules.Shared;

namespace Orquestra.Application.UseCases.Schedules.GetByCompanyId;

public interface IGetScheduleByCompanyId
{
    Task<List<ScheduleOutput>?> Execute(Guid companyId);
}