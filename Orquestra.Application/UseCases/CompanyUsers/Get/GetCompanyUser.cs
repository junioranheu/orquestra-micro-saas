using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.CompanyUsers.Get;

public sealed class GetCompanyUser(Context context) : IGetCompanyUser
{
    private readonly Context _context = context;

    public async Task<List<CompanyUserOutput>?> Execute(Guid companyId, Guid? userId = null)
    {
        var result = await _context.CompanyUsers.
                     Include(x => x.Users).
                     AsNoTracking().
                     Where(x =>
                        (companyId == Guid.Empty || x.CompanyId == companyId) &&
                        ((userId == Guid.Empty || userId == null) || x.UserId == userId)
                     ).
                     ToListAsync();

        var output = result.Adapt<List<CompanyUserOutput>>();

        return output;
    }
}