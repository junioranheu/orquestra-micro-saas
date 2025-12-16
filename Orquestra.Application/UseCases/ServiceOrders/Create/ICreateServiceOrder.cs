using Orquestra.Application.UseCases.ServiceOrders.Shared;

namespace Orquestra.Application.UseCases.ServiceOrders.Create;

public interface ICreateServiceOrder
{
    Task Execute(Guid userIdAuth, ServiceOrderInput input);
}