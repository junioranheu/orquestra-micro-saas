using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Companies.Get;

public sealed class GetCompany(Context context, IMapper map) : IGetCompany
{
    private readonly Context _context = context;
    private readonly IMapper _map = map;

    public async Task<CompanyOutput?> Execute(Guid companyId)
    {
        var result = await _context.Companies.
                     Include(x => x.CompanyUsers)!.ThenInclude(x => x.Users).
                     AsNoTracking().
                     Where(x => x.CompanyId == companyId).
                     FirstOrDefaultAsync();

        CompanyOutput? output = _map.Map<CompanyOutput>(result);

        return output;
    }

    public async Task<List<CompanyOutput>?> Execute()
    {
        var result = await _context.Companies.
                     Include(x => x.CompanyUsers)!.ThenInclude(x => x.Users).
                     AsNoTracking().
                     ToListAsync();

        List<CompanyOutput>? output = _map.Map<List<CompanyOutput>>(result);

        return output;
    }
}