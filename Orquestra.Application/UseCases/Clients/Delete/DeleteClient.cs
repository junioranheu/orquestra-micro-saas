using Microsoft.EntityFrameworkCore;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Clients.Delete;

public sealed class DeleteClient(Context context) : IDeleteClient
{
    private readonly Context _context = context;

    public async Task Execute(Guid userIdAuth, Guid clientId)
    {
        Client? client = await _context.Clients.
                         // AsNoTracking(). // Propositalmente sem AsNoTracking;
                         Where(x => x.ClientId == clientId).
                         FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundClient);

        CompanyUser? user = await _context.CompanyUsers.
                            AsNoTracking().
                            Where(x =>
                               x.CompanyId == client.CompanyId &&
                               x.UserId == userIdAuth &&
                               x.CompanyUserRole == CompanyUserRoleEnum.Administrator &&
                               x.Status == true
                            ).FirstOrDefaultAsync() ?? throw new UnauthorizedAccessException("Você não tem permissão para excluir este cliente.");

        client.Status = false;

        _context.Update(client);
        await _context.SaveChangesAsync();
    }
}