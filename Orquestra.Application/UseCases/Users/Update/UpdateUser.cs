using AutoMapper;
using Orquestra.Application.UseCases.Users.Base;
using Orquestra.Application.UseCases.Users.GetByEmail;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Users.Update;

public sealed class UpdateUser(Context context, IMapper map, IGetUserByEmail getUserByEmail) : UserBase(getUserByEmail), IUpdateUser
{
    private readonly Context _context = context;
    private readonly IMapper _map = map;

    public async Task<UserOutput> Execute(Guid userId, UserInput input)
    {
        await Validate(input, userId, isCreate: false);
        User user = await Update(input);

        UserOutput? output = _map.Map<UserOutput>(user);

        return output;
    }

    #region extras
    private async Task<User> Update(UserInput input)
    {
        User? user = _map.Map<User>(input);

        _context.Update(user);
        await _context.SaveChangesAsync();

        return user;
    }
    #endregion
}