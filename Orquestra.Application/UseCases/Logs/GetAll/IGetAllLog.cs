using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;

namespace Orquestra.Application.UseCases.Logs.GetAll
{
    public interface IGetAllLog
    {
        Task<(IEnumerable<Log> linq, int count)> Execute(PaginationInput pagination, Guid? userId);
    }
}