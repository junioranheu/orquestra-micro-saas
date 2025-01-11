using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Entities;

namespace Orquestra.Application.UseCases.Users.Get;

public interface IGetUser
{
    Task<(User? user, string passwordEncrypted)> Execute(UserInput input);
}