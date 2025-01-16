using Microsoft.EntityFrameworkCore;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.CompanyUsers.Get;

public sealed class GetCompanyUser(Context context) : IGetCompanyUser
{
    private readonly Context _context = context;

    public async Task<List<CompanyUser>?> Execute(Guid companyId, Guid userId)
    {
        var companyUsers = await _context.CompanyUsers.
                           Include(x => x.Users).
                           AsNoTracking().
                           Where(x =>
                              (companyId == Guid.Empty || x.CompanyId == companyId) &&
                              (userId == Guid.Empty || x.UserId == userId)
                           ).
                           ToListAsync();

        return companyUsers;
    }
}