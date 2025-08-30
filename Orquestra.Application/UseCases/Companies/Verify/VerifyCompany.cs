using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Verifications.Get;
using Orquestra.Application.UseCases.Verifications.Update;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Companies.Verify;

public sealed class VerifyCompany(
        Context context, 
        IGetVerification getVerification, 
        IUpdateVerification updateVerification
    ) : IVerifyCompany
{
    private readonly Context _context = context;
    private readonly IGetVerification _getVerification = getVerification;
    private readonly IUpdateVerification _updateVerification = updateVerification;

    public async Task Execute(string token)
    {
        Verification verification = await _getVerification.Execute(token, verificationType: VerificationTypeEnum.Company);

        var result = await _context.Companies.
                     AsNoTracking().
                     Where(x => x.CompanyId == verification.EntityId).
                     FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"O token {token} não pertence a nenhuma empresa.");

        // Salvar alteração;
        result.Status = true;

        _context.Update(result);
        await _context.SaveChangesAsync();

        // Atualizar status da veriicação;
        await _updateVerification.Execute(verificationId: verification.VerificationId);
    }
}