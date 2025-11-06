using Orquestra.Application.UseCases.Inventories.Shared;

namespace Orquestra.Application.UseCases.Inventories.Update;

public interface IUpdateInventory
{
    Task Execute(Guid userIdAuth, InventoryInput input);
}