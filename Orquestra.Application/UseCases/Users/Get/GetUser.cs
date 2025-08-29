using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Users.Get;

public sealed class GetUser(Context context) : IGetUser
{
    private readonly Context _context = context;

    public async Task<(UserOutput? output, string passwordEncrypted)> Execute(UserInput input)
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
            throw new UnauthorizedAccessException(SystemConsts.Warn_NeedToVerifyUser);
        }

        string password = result.Password;
        var output = result.Adapt<UserOutput>();

        return (output, password);
    }

    public async Task<UserOutput> Execute(Guid? userId, string? email = "", bool throwIfStatusFalse = true)
    {
        if (!string.IsNullOrEmpty(email))
        {
            email = email.Trim().ToLowerInvariant();
        }

        var result = await _context.Users.
                     AsNoTracking().
                     Where(x => 
                        (x.UserId == Guid.Empty || x.UserId == userId) &&
                        (string.IsNullOrEmpty(x.Email) || x.Email == email)
                     ).FirstOrDefaultAsync();

        if (result is null)
        {
            return new();
        }

        if (throwIfStatusFalse && !result.Status)
        {
            throw new UnauthorizedAccessException(SystemConsts.Warn_NeedToVerifyUser);
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