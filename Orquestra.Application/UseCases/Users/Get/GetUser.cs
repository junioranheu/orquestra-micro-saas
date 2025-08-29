using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Users.Get;

public sealed class GetUser(Context context) : IGetUser
{
    private readonly Context _context = context;

    public async Task<(User? user, string passwordEncrypted)> Execute(UserInput input)
    {
        input.Email = input.Email?.Trim().ToLowerInvariant();

        var result = await _context.Users.
                     AsNoTracking().
                     Where(x =>
                        (input.UserId == Guid.Empty || x.UserId == input.UserId) &&
                        (string.IsNullOrEmpty(input.Email) || x.Email == input.Email)
                     ).FirstOrDefaultAsync();

        if (result is null)
        {
            return (null, string.Empty);
        }

        if (!result.Status)
        {
            throw new UnauthorizedAccessException("A conta ainda não foi verificada ou está desativada. Verifique-a e tente novamente.");
        }

        string password = result.Password;
        result.Password = string.Empty;

        return (result, password);
    }

    public async Task<UserOutput> Execute(Guid userId, string email)
    {
        var result = await _context.Users.
                     AsNoTracking().
                     Where(x => x.UserId == userId && x.Status == true).
                     FirstOrDefaultAsync();

        if (result is null)
        {
            return new();
        }

        var output = result.Adapt<UserOutput>();

        return output;
    }

    public async Task<(IEnumerable<UserOutput> output, int count)> Execute(PaginationInput pagination)
    {
        var query = _context.Users.
                    AsNoTracking().
                    Where(x => x.Status == true).
                    OrderByDescending(x => x.CreatedDate);

        (IEnumerable<User> linq, int count) = await PagedQuery.Execute(query, pagination);
        var output = linq.Adapt<IEnumerable<UserOutput>>();

        return (output, count);
    }
}