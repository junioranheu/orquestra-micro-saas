using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.ServiceOrders.Delete;

public sealed class DeleteServiceOrder(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IDeleteServiceOrder
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task Execute(Guid userIdAuth, Guid serviceOrderId)
    {
        ServiceOrder? order = await _context.ServiceOrder.
                              // AsNoTracking(). // Propositalmente sem AsNoTracking;
                              Where(x => x.QuoteId == serviceOrderId).
                              FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundData);

        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: order.CompanyId, userId: userIdAuth, needCompanyAdmin: true);

        order.Status = false;
        _context.Update(order);
        await _context.SaveChangesAsync();
    }
}