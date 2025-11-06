using Mapster;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Inventories.Base;
using Orquestra.Application.UseCases.Inventories.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Inventories.Create;

public sealed class CreateInventory(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : InventoryBase(context, checkIfUserIsLinkedCompanyUser), ICreateInventory
{
    private readonly Context _context = context;

    public async Task Execute(Guid userIdAuth, InventoryInput input)
    {
        await Validate(input, userIdAuth, isCreate: true);
        await Save(input);
    }

    #region extras
    private async Task Save(InventoryInput input)
    {
        var inventory = input.Adapt<Inventory>();

        if (input.ImageFormFile is not null)
        {
            using MemoryStream ms = new();
            await input.ImageFormFile.CopyToAsync(ms);
            inventory.Image = ms.ToArray();
            inventory.ImageContentType = input.ImageFormFile.ContentType;
        }
        else
        {
            inventory.Image = null;
            inventory.ImageContentType = null;
        }

        await _context.AddAsync(inventory);
        await _context.SaveChangesAsync();
    }
    #endregion
}