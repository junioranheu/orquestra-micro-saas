namespace Orquestra.Application.UseCases.Users.RecoverPassword;

public interface IRecoverPasswordUser
{
    Task SendEmail(string email);
    Task Verify(string token);
}