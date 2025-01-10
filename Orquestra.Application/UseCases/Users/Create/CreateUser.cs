using AutoMapper;
using Orquestra.Application.UseCases.Users.GetByEmail;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Encrypt;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Users.Create;

public sealed class CreateUser(Context context, IMapper map, IGetUserByEmail getUserByEmail) : ICreateUser
{
    private readonly Context _context = context;
    private readonly IMapper _map = map;
    private readonly IGetUserByEmail _getUserByEmail = getUserByEmail;

    public async Task<UserOutput> Execute(UserInput input)
    {
        await Validations(input);
        User user = await SaveUser(input);

        UserOutput? output = _map.Map<UserOutput>(user);

        return output;
    }

    private async Task Validations(UserInput input)
    {
        (User? checkUserByEmail, string _) = await _getUserByEmail.Execute(input.Email);

        if (checkUserByEmail is not null)
        {
            throw new Exception("Já existe um usuário com esse e-mail");
        }
    }

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
}