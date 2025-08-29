using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Verifications.Get;
using Orquestra.Application.UseCases.Verifications.Update;
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
        var verification = await _getVerification.Execute(token, verificationType: VerificationTypeEnum.CompanyUser);

        var result = await _context.CompanyUsers.
                     AsNoTracking().
                     Where(x => x.CompanyUserId == verification.EntityId).
                     FirstOrDefaultAsync() ?? throw new Exception($"O token {token} não pertence a nenhum usuário.");

        if (!result.Status)
        {
            throw new Exception("Este token pertence a um um usuário desativado na base de dados.");
        }

        // Salvar alteração;
        result.Status = true;

        _context.Update(result);
        await _context.SaveChangesAsync();

        // Atualizar status da veriicação;
        await _updateVerification.Execute(verificationId: verification.VerificationId);
    }
}