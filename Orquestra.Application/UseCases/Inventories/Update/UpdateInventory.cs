using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Inventories.Base;
using Orquestra.Application.UseCases.Inventories.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Inventories.Update;

public sealed class UpdateInventory(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : InventoryBase(context, checkIfUserIsLinkedCompanyUser), IUpdateInventory
{
    private readonly Context _context = context;

    public async Task Execute(Guid userIdAuth, InventoryInput input)
    {
        Inventory? inventory = await _context.Inventories.
                               // AsNoTracking(). // Propositalmente sem AsNoTracking;
                               Where(x => x.InventoryId == input.InventoryId).
                               FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundData);

        await Validate(input, userIdAuth, isCreate: false);
        await Update(input, inventory);
    }

    #region extras
    private async Task Update(InventoryInput input, Inventory inventory)
    {
        inventory.Name = input.Name!;
        inventory.Description = input.Description;
        inventory.Quantity = input.Quantity.GetValueOrDefault();
        inventory.UnitPrice = input.UnitPrice.GetValueOrDefault();

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

        _context.Update(inventory);
        await _context.SaveChangesAsync();
    }
    #endregion
}