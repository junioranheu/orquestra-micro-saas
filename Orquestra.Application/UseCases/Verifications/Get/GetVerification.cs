using Microsoft.EntityFrameworkCore;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Verifications.Get;

public sealed class GetVerification(Context context) : IGetVerification
{
    private readonly Context _context = context;

    public async Task<Verification> Execute(string token, VerificationTypeEnum verificationType)
    {
        var verification = await _context.Verifications.
                           AsNoTracking().
                           Where(x => x.Token == token && x.VerificationType == verificationType && x.Status == true).
                           FirstOrDefaultAsync() ?? throw new InvalidOperationException(SystemConsts.Warn_VerifyToken_Invalid);

        if (verification.Used)
        {
            throw new InvalidOperationException("Este token já foi utilizado em uma verificação anteriormente.");
        }

        return verification;
    }

    public async Task<Verification> Execute(Guid verificationId)
    {
        var verification = await _context.Verifications.
                           AsNoTracking().
                           Where(x => x.VerificationId == verificationId && x.Status == true).
                           FirstOrDefaultAsync() ?? throw new InvalidOperationException(SystemConsts.Warn_VerifyToken_Invalid);

        if (verification.Used)
        {
            throw new InvalidOperationException("Este token já foi verificado anteriormente.");
        }

        return verification;
    }
}