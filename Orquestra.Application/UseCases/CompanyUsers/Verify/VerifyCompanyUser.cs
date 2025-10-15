using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Verifications.Get;
using Orquestra.Application.UseCases.Verifications.Update;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.CompanyUsers.Verify;

public sealed class VerifyCompanyUser(
        Context context, 
        IGetVerification getVerification,
        IUpdateVerification updateVerification
    ) : IVerifyCompanyUser
{
    private readonly Context _context = context;
    private readonly IGetVerification _getVerification = getVerification;
    private readonly IUpdateVerification _updateVerification = updateVerification;

    public async Task Execute(string token)
    {
        Verification verification = await _getVerification.Execute(token, verificationType: VerificationTypeEnum.CompanyUser);

        var result = await _context.CompanyUsers.
                     // AsNoTracking(). // Propositalmente sem AsNoTracking;
                     Where(x => x.CompanyUserId == verification.EntityId).
                     FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"O token {token} não pertence a nenhum usuário.");

        // Salvar alteração;
        result.Status = true;
        result.IsCurrentMainCompanyUser = true;

        _context.Update(result);
        await _context.SaveChangesAsync();

        // Atualizar status da verificação;
        await _updateVerification.Execute(verificationId: verification.VerificationId);
    }
}