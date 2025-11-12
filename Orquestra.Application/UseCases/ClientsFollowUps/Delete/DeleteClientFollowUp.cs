using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.ClientsFollowUps.Delete;

public sealed class DeleteClientFollowUp(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IDeleteClientFollowUp
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task Execute(Guid userIdAuth, Guid clientFollowUpId)
    {
        ClientFollowUp? clientFollowUp = await _context.ClientsFollowUps.
            // AsNoTracking(). // Propositalmente sem AsNoTracking;
            Where(x => x.ClientFollowUpId == clientFollowUpId && x.Status == true).
            FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundData);

        Client? client = await _context.Clients.AsNoTracking().Where(x => x.ClientId == clientFollowUp.ClientId && x.Status == true).FirstOrDefaultAsync() ?? throw new ArgumentException(SystemConsts.Warnings.NotFoundClient);

        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: client.CompanyId, userId: userIdAuth, needCompanyAdmin: true);

        clientFollowUp.Status = false;
        _context.Update(clientFollowUp);
        await _context.SaveChangesAsync();
    }
}