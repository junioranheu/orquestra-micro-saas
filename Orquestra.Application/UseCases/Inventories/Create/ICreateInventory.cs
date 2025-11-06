using Orquestra.Application.UseCases.Inventories.Shared;

namespace Orquestra.Application.UseCases.Inventories.Create;

public interface ICreateInventory
{
    Task Execute(Guid userIdAuth, InventoryInput input);
}