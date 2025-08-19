using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Users.Base;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Encrypt;

namespace Orquestra.Application.UseCases.Users.Update;

public sealed class UpdateUser(Context context,IGetUser getUser) : UserBase(getUser), IUpdateUser
{
    private readonly Context _context = context;

    public async Task<UserOutput> Execute(Guid userId, UserInput input)
    {
        await Validate(input, userId, isCreate: false);
        User user = await Update(userId, input);

        var output = user.Adapt<UserOutput>();

        return output;
    }

    #region extras
    private async Task<User> Update(Guid userId, UserInput input)
    {
        User? user = await _context.Users.AsNoTracking().Where(x => x.UserId == userId).FirstOrDefaultAsync() ?? throw new Exception("Usuário não encontrado.");

        user.FullName = input.FullName ?? user.FullName;
        user.Email = input.Email ?? user.Email;
        user.Password = !string.IsNullOrEmpty(input.Password) ? EncryptPassword(input.Password) : user.Password;

        _context.Update(user);
        await _context.SaveChangesAsync();

        return user;
    }
    #endregion
}