using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.CompanyUsers.Get;

public sealed class GetCompanyUser(Context context, IMapper map) : IGetCompanyUser
{
    private readonly Context _context = context;
    private readonly IMapper _map = map;

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

        var output = _map.Map<List<CompanyUserOutput>>(result);

        return output;
    }
}