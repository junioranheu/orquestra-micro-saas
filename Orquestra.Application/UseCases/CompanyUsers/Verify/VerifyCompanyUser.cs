using Microsoft.EntityFrameworkCore;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.CompanyUsers.Verify;

public sealed class VerifyCompanyUser(Context context) : IVerifyCompanyUser
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
            throw new Exception("Este token pertence a um um usuário desativado na base de dados.");
        }

        if (result.IsAccountVerified)
        {
            throw new Exception("O usuário já foi verificado.");
        }

        result.IsAccountVerified = true;

        _context.Update(result);
        await _context.SaveChangesAsync();
    }
}