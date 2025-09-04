using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Companies.Get;

public sealed class GetCompany(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IGetCompany
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task<CompanyOutput> Execute(Guid userIdAuth, Guid companyId, bool throwIfStatusFalse = true)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: false);

        var result = await _context.Companies.
                     Include(x => x.CompanyUsers)!.ThenInclude(x => x.User).
                     AsNoTracking().
                     Where(x => x.CompanyId == companyId).
                     FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warn_NotFound_Company);

        if (throwIfStatusFalse && !result.Status)
        {
            throw new InvalidOperationException(SystemConsts.Warn_NeedToVerify_Company);
        }

        var output = result.Adapt<CompanyOutput>();

        return output;
    }

    public async Task<List<CompanyOutput>?> Execute()
    {
        var result = await _context.Companies.
                     Include(x => x.CompanyUsers)!.ThenInclude(x => x.User).
                     AsNoTracking().
                     Where(x => x.Status == true).
                     ToListAsync();

        var output = result.Adapt<List<CompanyOutput>>();

        return output;
    }

    public async Task<List<CompanyOutput>?> Execute(Guid userId)
    {
        var companiesIds = await _context.CompanyUsers.
                           AsNoTracking().
                           Where(x => x.UserId == userId && x.Status == true).
                           Select(x => x.CompanyId).
                           Distinct().
                           ToListAsync();

        if (companiesIds.Count == 0)
        {
            return [];
        }

        var result = await _context.Companies.
                     Include(x => x.CompanyUsers)!.ThenInclude(x => x.User).
                     AsNoTracking().
                     Where(x => companiesIds.Contains(x.CompanyId) && x.Status == true).
                     ToListAsync();

        var output = result.Adapt<List<CompanyOutput>>();

        return output;
    }
}