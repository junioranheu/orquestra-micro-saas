using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;

public sealed class GetCompanyUserByCompanyId(Context context) : IGetCompanyUserByCompanyId
{
    private readonly Context _context = context;

    public async Task<List<CompanyUserOutput>?> Execute(Guid companyId, Guid? userId = null)
    {
        var result = await _context.CompanyUsers.
                     Include(x => x.User).
                     AsNoTracking().
                     Where(x =>
                        (companyId == Guid.Empty || x.CompanyId == companyId) &&
                        ((userId == Guid.Empty || userId == null) || x.UserId == userId) &&
                        x.Status == true
                     ).
                     GroupBy(x => new { x.CompanyId, x.UserId, x.CompanyUserRole }).
                     Select(g => g.FirstOrDefault()).
                     ToListAsync();

        var output = result.Adapt<List<CompanyUserOutput>>();

        return output;
    }
}