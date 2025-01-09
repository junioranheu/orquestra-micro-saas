using Orquestra.Domain.Entities;

namespace Orquestra.Application.UseCases.Users.GetByUserNameOrEmail
{
    public interface IGetUserByUserNameOrEmail
    {
        Task<(User? user, string passwordEncrypted)> Execute(string login);
    }
}