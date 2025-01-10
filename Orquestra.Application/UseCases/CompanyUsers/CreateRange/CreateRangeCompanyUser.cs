using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.CompanyUsers.CreateRange;

public sealed class CreateRangeCompanyUser(Context context) : ICreateRangeCompanyUser
{
    private readonly Context _context = context;

    public async Task Execute(List<CompanyUser> companyUsers)
    {
        await _context.AddRangeAsync(companyUsers);
        await _context.SaveChangesAsync();
    }
}