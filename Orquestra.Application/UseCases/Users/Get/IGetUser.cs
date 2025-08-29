using Orquestra.Application.UseCases.Shared;
using Orquestra.Application.UseCases.Users.Shared;

namespace Orquestra.Application.UseCases.Users.Get;

public interface IGetUser
{
    Task<(UserOutput? output, string passwordEncrypted)> Execute(UserInput input);
    Task<UserOutput> Execute(Guid? userId, string? email = "", bool throwIfStatusFalse = true);
    Task<(IEnumerable<UserOutput> output, int count)> Execute(PaginationInput pagination);
}