using Orquestra.Domain.Entities;

namespace Orquestra.Application.UseCases.Auth.RecoverPassword;

public interface IRecoverPasswordUser
{
    Task SendEmail(string email);
    Task Verify(string token);
}