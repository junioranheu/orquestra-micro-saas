using Orquestra.Application.UseCases.Verifications.Get;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Verifications.Update;

public sealed class UpdateVerification(Context context, IGetVerification get) : IUpdateVerification
{
    private readonly Context _context = context;
    private readonly IGetVerification _get = get;

    public async Task Execute(Guid verificationId)
    {
        Verification verification = await _get.Execute(verificationId);

        verification.Used = true;

        _context.ChangeTracker.Clear();
        _context.Update(verification);
        await _context.SaveChangesAsync();
    }
}