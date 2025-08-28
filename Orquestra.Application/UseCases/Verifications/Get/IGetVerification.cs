using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Verifications.Get;

public interface IGetVerification
{
    Task<Verification> Execute(string token, VerificationTypeEnum verificationType);
    Task<Verification> Execute(Guid verificationId);
}