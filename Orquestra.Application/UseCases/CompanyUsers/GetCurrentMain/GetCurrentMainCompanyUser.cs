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
        var companyUser = await _context.CompanyUsers.
                          AsNoTracking().
                          Where(x => 
                             x.UserId == userId && 
                             x.IsCurrentMainCompanyUser == true &&
                             x.Status == true
                          ).FirstOrDefaultAsync();

        if (companyUser is null)
        {
            return null;
        }

        var company = await _context.Companies.AsNoTracking().Where(x => x.CompanyId == companyUser.CompanyId).FirstOrDefaultAsync();

        if (company is null)
        {
            return null;
        }

        var output = company.Adapt<CompanyOutput>();

        return output;
    }
}