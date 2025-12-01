using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.ClientsFollowUps.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.ClientsFollowUps.GetAllByCompanyId;

public sealed class GetAllClientFollowUpByCompanyId(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IGetAllClientFollowUpByCompanyId
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task<(IEnumerable<ClientFollowUpOutput> output, int count)> Execute(PaginationInput pagination, ClientFollowUpInput input, Guid userIdAuth, Guid companyId)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: false);

        var query = _context.ClientsFollowUps.
                    AsNoTracking().
                    Where(x =>
                        x.CompanyId == companyId &&
                        x.Status == true &&
                        ((input.ClientId == null || input.ClientId == Guid.Empty) || x.ClientId == input.ClientId) &&
                        (input.ClientFollowUpStatus <= 0 || x.ClientFollowUpStatus == input.ClientFollowUpStatus)
                    ).OrderByDescending(x => x.CreatedDate);

        (IEnumerable<ClientFollowUp> linq, int count) = await PagedQuery.Execute(query, pagination);
        var output = linq.Adapt<List<ClientFollowUpOutput>>();

        return (output, count);
    }
}