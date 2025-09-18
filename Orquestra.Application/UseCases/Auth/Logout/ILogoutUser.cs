namespace Orquestra.Application.UseCases.Auth.Logout;

public interface ILogoutUser
{
    Task Execute(Guid userIdAuth);
}