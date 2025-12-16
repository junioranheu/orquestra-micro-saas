using Orquestra.Application.UseCases.ServiceOrders.Shared;

namespace Orquestra.Application.UseCases.ServiceOrders.Update;

public interface IUpdateServiceOrder
{
    Task Execute(Guid userIdAuth, ServiceOrderInput input);
}