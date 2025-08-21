using Microsoft.EntityFrameworkCore;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Companies.Verify;

public sealed class VerifyCompany(Context context) : IVerifyCompany
{
    private readonly Context _context = context;

    public async Task Execute(string token)
    {
        var result = await _context.CompanyUsers.
                     AsNoTracking().
                     Where(x => x.VerifyToken == token).
                     FirstOrDefaultAsync() ?? throw new Exception($"O token {token} não pertence a nenhum usuário.");

        if (!result.Status)
        {
            throw new Exception($"Este token pertence a um usuário desativado.");
        }

        if (result.IsAccountVerified)
        {
            throw new Exception($"Este usuário já foi verificado.");
        }

        result.IsAccountVerified = true;

        _context.Update(result);
        await _context.SaveChangesAsync();
    }
}