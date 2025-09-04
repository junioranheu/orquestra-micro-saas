using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Clients.Get;

public sealed class GetClient(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IGetClient
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task<ClientOutput?> Execute(Guid userIdAuth, Guid clientId)
    {
        var result = await _context.Clients.
                     Include(x => x.Company).
                     AsNoTracking().
                     Where(x => x.ClientId == clientId && x.Status == true).
                     FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warn_NotFound_Client);

        Guid companyId = result.CompanyId;
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: false);

        var output = result.Adapt<ClientOutput>();

        return output;
    }
}