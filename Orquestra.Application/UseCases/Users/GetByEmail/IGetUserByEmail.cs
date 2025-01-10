using Orquestra.Domain.Entities;

namespace Orquestra.Application.UseCases.Users.GetByEmail
{
    public interface IGetUserByEmail
    {
        Task<(User? user, string passwordEncrypted)> Execute(string login);
    }
}