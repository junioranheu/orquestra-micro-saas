namespace Orquestra.Application.UseCases.ServiceOrders.Delete;

public interface IDeleteServiceOrder
{
    Task Execute(Guid userIdAuth, Guid serviceOrderId);
}