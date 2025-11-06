using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Inventories.Delete;

public sealed class DeleteInventory(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IDeleteInventory
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task Execute(Guid userIdAuth, Guid inventoryId)
    {
        Inventory? inventory = await _context.Inventories.
                              // AsNoTracking(). // Propositalmente sem AsNoTracking;
                              Where(x => x.InventoryId == inventoryId && x.Status == true).
                              FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundData);

        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: inventory.CompanyId, userId: userIdAuth, needCompanyAdmin: true);

        inventory.Status = false;
        _context.Update(inventory);
        await _context.SaveChangesAsync();
    }
}