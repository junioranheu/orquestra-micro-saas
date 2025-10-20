using Orquestra.Application.UseCases.Verifications.Get;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Verifications.Update;

public sealed class UpdateVerification(Context context, IGetVerification get) : IUpdateVerification
{
    private readonly Context _context = context;
    private readonly IGetVerification _get = get;

    public async Task Execute(Guid verificationId)
    {
        Verification verification = await _get.Execute(verificationId);

        if (verification.ExpirationDate is not null && verification.ExpirationDate < GetDate())
        {
            throw new InvalidOperationException($"A data de expiração desse convite não está mais válida. Expirado em: {GetDateDetails(verification.ExpirationDate)}.");
        }

        verification.Used = true;

        _context.Update(verification);
        await _context.SaveChangesAsync();
    }
}