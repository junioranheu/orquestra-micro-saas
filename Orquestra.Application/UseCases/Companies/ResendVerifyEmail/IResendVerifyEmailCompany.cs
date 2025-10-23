namespace Orquestra.Application.UseCases.Companies.ResendVerifyEmail;

public interface IResendVerifyEmailCompany
{
    Task Execute(Guid userIdAuth, Guid companyId);
}