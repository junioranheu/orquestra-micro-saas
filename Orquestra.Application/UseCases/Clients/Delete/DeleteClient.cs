using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Clients.Delete;

public sealed class DeleteClient(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IDeleteClient
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task Execute(Guid userIdAuth, Guid clientId)
    {
        Client? client = await _context.Clients.
                         // AsNoTracking(). // Propositalmente sem AsNoTracking;
                         Where(x => x.ClientId == clientId).
                         FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundClient);

        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: client.CompanyId, userId: userIdAuth, needCompanyAdmin: true);

        client.Status = false;
        _context.Update(client);
        await _context.SaveChangesAsync();
    }
}