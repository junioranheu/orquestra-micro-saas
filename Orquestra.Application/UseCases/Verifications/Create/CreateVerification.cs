using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Verifications.Create;

public sealed class CreateVerification(Context context) : ICreateVerification
{
    private readonly Context _context = context;

    public async Task<Verification> Execute<T>(Guid entityId, VerificationTypeEnum verificationType, string? reference = "")
    {
        Verification verification = new()
        {
            Token = GenerateSafeToken32Bytes(urlSafe: true),
            EntityType = typeof(T).Name,
            EntityId = entityId,
            VerificationType = verificationType,
            Used = false,
            Reference = reference
        };

        await _context.AddAsync(verification);
        await _context.SaveChangesAsync();

        return verification;
    }
}