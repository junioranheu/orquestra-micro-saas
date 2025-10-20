using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Verifications.Create;

public interface ICreateVerification
{
    Task<Verification> Execute<T>(Guid entityId, VerificationTypeEnum verificationType, string? reference = "", DateTime? expirationDate = null);
}