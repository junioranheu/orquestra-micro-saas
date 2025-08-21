using Mapster;
using Orquestra.Application.UseCases.Users.Base;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Encrypt;

namespace Orquestra.Application.UseCases.Users.Create;

public sealed class CreateUser(Context context, IGetUser getUser) : UserBase(getUser), ICreateUser
{
    private readonly Context _context = context;

    public async Task<UserOutput> Execute(UserInput input)
    {
        await Validate(input, userIdAuth: Guid.Empty, isCreate: true);
        User user = await Save(input);

        var output = user.Adapt<UserOutput>(); 

        return output;
    }

    #region extras
    private async Task<User> Save(UserInput input)
    {
        if (string.IsNullOrEmpty(input.FullName) || string.IsNullOrEmpty(input.Email) || string.IsNullOrEmpty(input.Password))
        {
            throw new Exception("Os dados do usuário não podem ser nulos.");
        }

        User user = new()
        {
            FullName = input.FullName,
            Email = input.Email,
            Password = EncryptPassword(input.Password),
            Role = UserRoleEnum.Common
        };

        await _context.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }
    #endregion
}