using Mapster;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.ServiceOrders.Base;
using Orquestra.Application.UseCases.ServiceOrders.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.ServiceOrders.Create;

public sealed class CreateServiceOrder(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : ServiceOrderBase(checkIfUserIsLinkedCompanyUser), ICreateServiceOrder
{
    private readonly Context _context = context;

    public async Task Execute(Guid userIdAuth, ServiceOrderInput input)
    {
        await Validate(input, userIdAuth);
        await Save(input);
    }

    #region extras
    private async Task Save(ServiceOrderInput input)
    {
        var quote = input.Adapt<ServiceOrder>();

        await _context.AddAsync(quote);
        await _context.SaveChangesAsync();
    }
    #endregion
}