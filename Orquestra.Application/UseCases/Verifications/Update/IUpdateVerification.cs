namespace Orquestra.Application.UseCases.Verifications.Update;

public interface IUpdateVerification
{
    Task Execute(Guid verificationId);
}