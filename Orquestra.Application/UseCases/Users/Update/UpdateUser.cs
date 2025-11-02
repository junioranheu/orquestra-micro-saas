using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Users.Base;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Encrypt;

namespace Orquestra.Application.UseCases.Users.Update;

public sealed class UpdateUser(Context context, IGetUser getUser) : UserBase(getUser), IUpdateUser
{
    private readonly Context _context = context;

    public async Task Execute(Guid userIdAuth, UserInput input)
    {
        (User user, UserInput inputNormalized, bool hasChangedPassword) = await Get(userIdAuth, input);
        await Validate(input: inputNormalized, userIdAuth, isCreate: false, hasChangedPassword);
        await Update(user, hasChangedPassword);
    }

    #region extras
    private async Task<(User user, UserInput inputNormalized, bool hasChangedPassword)> Get(Guid userIdAuth, UserInput input)
    {
        User? user = await _context.Users.
                     // AsNoTracking(). // Propositalmente sem AsNoTracking;
                     Where(x => x.UserId == userIdAuth && x.Status == true).
                     FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundUser);

        user.FullName = !string.IsNullOrEmpty(input.FullName) ? input.FullName : user.FullName;
        user.Email = !string.IsNullOrEmpty(input.Email) ? input.Email : user.Email;

        bool hasChangedPassword = !string.IsNullOrEmpty(input.Password);

        if (hasChangedPassword)
        {
            user.Password = input.Password!;
        }

        var inputNormalized = user.Adapt<UserInput>();

        return (user, inputNormalized, hasChangedPassword);
    }

    private async Task Update(User user, bool hasChangedPassword)
    {
        if (hasChangedPassword)
        {
            user.Password = EncryptPassword(user.Password);
        }

        _context.Update(user);
        await _context.SaveChangesAsync();
    }
    #endregion
}