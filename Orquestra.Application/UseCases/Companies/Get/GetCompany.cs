using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Companies.Get;

public sealed class GetCompany(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IGetCompany
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task<CompanyOutput?> Execute(Guid userId, Guid companyId)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId, needCompanyAdmin: false);

        var result = await _context.Companies.
                     Include(x => x.CompanyUsers)!.ThenInclude(x => x.User).
                     AsNoTracking().
                     Where(x => x.CompanyId == companyId).
                     FirstOrDefaultAsync();

        var output = result.Adapt<CompanyOutput>();

        return output;
    }

    public async Task<List<CompanyOutput>?> Execute()
    {
        var result = await _context.Companies.
                     Include(x => x.CompanyUsers)!.ThenInclude(x => x.User).
                     AsNoTracking().
                     ToListAsync();

        var output = result.Adapt<List<CompanyOutput>>();

        return output;
    }
}