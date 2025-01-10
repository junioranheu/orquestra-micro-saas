using Microsoft.EntityFrameworkCore;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Users.GetByEmail;

public sealed class GetUserByEmail(Context context) : IGetUserByEmail
{
    private readonly Context _context = context;

    public async Task<(User? user, string passwordEncrypted)> Execute(string login)
    {
        var user = await _context.Users.
                   AsNoTracking().
                   Where(x => x.Email == login).
                   FirstOrDefaultAsync();

        if (user is null)
        {
            return (null, string.Empty);
        }

        return (user, user.Password);
    }
}