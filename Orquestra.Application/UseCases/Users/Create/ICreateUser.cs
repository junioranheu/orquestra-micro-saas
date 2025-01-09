using Orquestra.Application.UseCases.Users.Shared;

namespace Orquestra.Application.UseCases.Users.Create;

public interface ICreateUser
{
    Task<UserOutput> Execute(UserInput input);
}