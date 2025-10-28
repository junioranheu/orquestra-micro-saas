using Orquestra.Application.UseCases.Logs.Shared;
using Orquestra.Application.UseCases.Shared;

namespace Orquestra.Application.UseCases.Logs.GetNotification;

public interface IGetNotificationLog
{
    Task<(List<LogNotificationOutput> output, int count)> Execute(PaginationInput pagination, Guid userIdAuth, bool isDashboard = false);
}