using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.ServiceOrders.Base;
using Orquestra.Application.UseCases.ServiceOrders.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.ServiceOrders.Update;

public sealed class UpdateServiceOrder(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : ServiceOrderBase(checkIfUserIsLinkedCompanyUser), IUpdateServiceOrder
{
    private readonly Context _context = context;

    public async Task Execute(Guid userIdAuth, ServiceOrderInput input)
    {
        ServiceOrder? order = await _context.ServiceOrder.
                              // AsNoTracking(). // Propositalmente sem AsNoTracking;
                              Where(x => x.QuoteId == input.QuoteId).
                              FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundData);

        await Validate(input, userIdAuth);
        await Update(input, order);
    }

    #region extras
    private async Task Update(ServiceOrderInput input, ServiceOrder order)
    {
        order.ClientId = input.ClientId.GetValueOrDefault();
        order.Title = input.Title;
        order.Observation = input.Observation;
        order.ExecutionDate = input.ExecutionDate.GetValueOrDefault();
        order.ServiceOrderStatus = input.ServiceOrderStatus.GetValueOrDefault();

        _context.Update(order);
        await _context.SaveChangesAsync();
    }
    #endregion
}