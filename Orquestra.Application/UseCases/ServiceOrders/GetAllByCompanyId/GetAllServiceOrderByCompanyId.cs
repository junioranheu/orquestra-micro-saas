using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.ServiceOrders.Shared;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.ServiceOrders.GetAllByCompanyId;

public sealed class GetAllServiceOrderByCompanyId(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IGetAllServiceOrderByCompanyId
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task<(IEnumerable<ServiceOrderOutput> output, int count)> Execute(PaginationInput pagination, ServiceOrderInput input, Guid userIdAuth, Guid companyId)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: false);

        var query = _context.ServiceOrders.
                    Include(x => x.Client).
                    Include(x => x.Company).
                    AsNoTracking().
                    Where(x =>
                        x.CompanyId == companyId &&
                        ((input.ServiceOrderId == null || input.ServiceOrderId == Guid.Empty) || x.ServiceOrderId == input.ServiceOrderId) &&
                        ((input.ClientId == null || input.ClientId == Guid.Empty) || x.ClientId == input.ClientId) &&
                        ((input.QuoteId == null || input.QuoteId == Guid.Empty) || x.QuoteId == input.QuoteId) &&
                        (string.IsNullOrEmpty(input.Title) || x.Title!.ToLower().Contains(input.Title.ToLower())) &&
                        (string.IsNullOrEmpty(input.Observation) || x.Observation!.ToLower().Contains(input.Observation!.ToLower())) &&
                        (input.ExecutionDate == null || input.ExecutionDate == DateTime.MinValue || (x.ExecutionDate >= input.ExecutionDate && x.ExecutionDate < input.ExecutionDate.Value.Date.AddDays(1))) &&
                        x.Status == true
                    ).
                    OrderByDescending(x => x.LastModificationDate ?? DateTime.MinValue).
                    ThenByDescending(x => x.CreatedDate);

        (IEnumerable<ServiceOrder> result, int count) = await PagedQuery.Execute(query, pagination);

        var output = result.Adapt<List<ServiceOrderOutput>>();

        return (output, count);
    }
}