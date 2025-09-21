using Orquestra.Application.UseCases.Schedules.Shared;

namespace Orquestra.Application.UseCases.Schedules.GetAllByCompanyId;

public interface IGetScheduleByCompanyId
{
    Task<List<ScheduleOutput>?> Execute(Guid userIdAuth, Guid companyId, int? year, int? month);
}