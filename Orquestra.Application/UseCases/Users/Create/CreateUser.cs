using AutoMapper;
using Orquestra.Application.UseCases.Users.Base;
using Orquestra.Application.UseCases.Users.GetByEmail;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Encrypt;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Users.Create;

public sealed class CreateUser(Context context, IMapper map, IGetUserByEmail getUserByEmail) : UserBase(getUserByEmail), ICreateUser
{
    private readonly Context _context = context;
    private readonly IMapper _map = map;

    public async Task<UserOutput> Execute(UserInput input)
    {
        await Validate(input, isCreate: true);
        User user = await SaveUser(input);

        UserOutput? output = _map.Map<UserOutput>(user);

        return output;
    }

    #region extras
    private async Task<User> SaveUser(UserInput input)
    {
        DateTime date = GetDate();
        const int codeValidityThreshold = 7;

        User user = new()
        {
            FullName = input.FullName,
            Email = input.Email,
            Password = EncryptPassword(input.Password),
            Role = UserRoleEnum.Common,
            ChangePasswordCode = GetRandomString(22, false),
            ChangePasswordCodeValidity = date.AddDays(codeValidityThreshold)
        };

        await _context.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }
    #endregion
}