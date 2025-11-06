using Orquestra.Application.UseCases.Schedules.Shared;

namespace Orquestra.Application.UseCases.Schedules.GetAllByCompanyId;

public interface IGetAllScheduleByCompanyId
{
    Task<List<ScheduleOutput>?> Execute(Guid userIdAuth, Guid companyId, int? year, int? month, bool? getOnlyNearbyDates = false);
}