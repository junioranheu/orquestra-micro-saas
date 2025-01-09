using Orquestra.Domain.Entities;

namespace Orquestra.Application.UseCases.Logs.Create
{
    public interface ICreateLog
    {
        Task Execute(Log input);
    }
}