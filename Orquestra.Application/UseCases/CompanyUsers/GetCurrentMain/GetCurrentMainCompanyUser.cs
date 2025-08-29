using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.CompanyUsers.GetCurrentMain;

public sealed class GetCurrentMainCompanyUser(Context context) : IGetCurrentMainCompanyUser
{
    private readonly Context _context = context;

    public async Task<CompanyOutput?> Execute(Guid userId)
    {
        var output = await _context.CompanyUsers.
                     AsNoTracking().
                     Where(x => x.UserId == userId && x.IsCurrentMainCompanyUser && x.Status).
                     Select(x => x.Company.Adapt<CompanyOutput>()).
                     FirstOrDefaultAsync();

        return output;
    }
}