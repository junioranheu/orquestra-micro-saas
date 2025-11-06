namespace Orquestra.Application.UseCases.Inventories.Delete;

public interface IDeleteInventory
{
    Task Execute(Guid userIdAuth, Guid inventoryId);
}