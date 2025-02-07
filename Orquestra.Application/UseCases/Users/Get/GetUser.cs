using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Users.Get;

public sealed class GetUser(Context context) : IGetUser
{
    private readonly Context _context = context;

    public async Task<(User? user, string passwordEncrypted)> Execute(UserInput input)
    {
        var result = await _context.Users.
                     AsNoTracking().
                     Where(x =>
                        (input.UserId == Guid.Empty || x.UserId == input.UserId) &&
                        (string.IsNullOrEmpty(input.Email) || x.Email == input.Email)
                     ).
                     FirstOrDefaultAsync();

        if (result is null)
        {
            return (null, string.Empty);
        }

        string password = result.Password;
        result.Password = string.Empty;

        return (result, result.Password);
    }
}