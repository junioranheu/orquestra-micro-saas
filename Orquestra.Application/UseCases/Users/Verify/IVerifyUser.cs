namespace Orquestra.Application.UseCases.Users.Verify;

public interface IVerifyUser
{
    Task Execute(string token);
}