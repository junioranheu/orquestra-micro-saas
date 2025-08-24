using Orquestra.Application.UseCases.Shared;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Entities;

namespace Orquestra.Application.UseCases.Users.Get;

public interface IGetUser
{
    Task<(User? user, string passwordEncrypted)> Execute(UserInput input);
    Task<UserOutput> Execute(Guid userId);
    Task<(IEnumerable<UserOutput> output, int count)> Execute(PaginationInput pagination);
}