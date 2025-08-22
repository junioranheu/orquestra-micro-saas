using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;

namespace Orquestra.Application.UseCases.Logs.Get
{
    public interface IGetLog
    {
        Task<(IEnumerable<Log> output, int count)> Execute(PaginationInput pagination, Guid? userId);
    }
}