using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

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

        FillModulesStr([output]);
        await GetAmounfOfClients([output]);

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

        FillModulesStr(output);
        await GetAmounfOfClients(output);

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

        FillModulesStr(output);
        await GetAmounfOfClients(output);

        return output;
    }

    #region extras
    private static void FillModulesStr(List<CompanyOutput>? output)
    {
        if (output is null || output.Count == 0)
        {
            return;
        }

        foreach (var company in output)
        {
            if (company.Modules is null || company.Modules.Length == 0)
            {
                continue;
            }

            List<string> modulesStr = [];

            foreach (var module in company.Modules)
            {
                modulesStr.Add(GetEnumDesc(module));
            }

            company.ModulesStr = modulesStr;
        }
    }

    private async Task GetAmounfOfClients(List<CompanyOutput>? output)
    {
        if (output is null || output.Count == 0)
        {
            return;
        }

        List<Guid> companyIds = [.. output.Select(c => c.CompanyId)];

        var aea = await _context.Clients.ToListAsync();

        var clientsCount = await _context.Clients.
                           AsNoTracking().
                           Where(c => companyIds.Contains(c.CompanyId)).
                           GroupBy(c => c.CompanyId).
                           Select(g => new { CompanyId = g.Key, Count = g.Count() }).
                           ToListAsync();

        if (clientsCount is null || clientsCount.Count == 0)
        {
            return;
        }

        // Dicionário pra lookup O(1);
        Dictionary<Guid, int> countDict = clientsCount.ToDictionary(x => x.CompanyId, x => x.Count);

        foreach (var company in output)
        {
            company.AmountOfClients = countDict.TryGetValue(company.CompanyId, out int count) ? count : 0;
        }
    }
    #endregion
}