using Microsoft.EntityFrameworkCore;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Companies.Get;

public sealed class GetCompany(Context context) : IGetCompany
{
    private readonly Context _context = context;

    public async Task<Company?> Execute(Guid companyId)
    {
        var company = await _context.Companies.
                      Include(x => x.CompanyUsers).
                      AsNoTracking().
                      Where(x => x.CompanyId == companyId).
                      FirstOrDefaultAsync();

        return company;
    }
}