using Orquestra.Application.UseCases.Users.Shared;

namespace Orquestra.Application.UseCases.Users.Update;

public interface IUpdateUser
{
    Task<UserOutput> Execute(Guid userIdAuth, UserInput input);
}