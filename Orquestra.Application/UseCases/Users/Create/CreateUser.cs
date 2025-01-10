using AutoMapper;
using Orquestra.Application.UseCases.Users.GetByUserNameOrEmail;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;
using static Orquestra.Utils.Fixtures.Encrypt;

namespace Orquestra.Application.UseCases.Users.Create;

public sealed class CreateUser(Context context, IMapper map, IGetUserByUserNameOrEmail getUserByUserNameOrEmail) : ICreateUser
{
    private readonly Context _context = context;
    private readonly IMapper _map = map;
    private readonly IGetUserByUserNameOrEmail _getUserByUserNameOrEmail = getUserByUserNameOrEmail;

    public async Task<UserOutput> Execute(UserInput input)
    {
        await Validations(input);
        User user = await SaveUser(input);
        await SaveUserRole(input, user.UserId);

        UserOutput? output = _map.Map<UserOutput>(user);

        return output;
    }

    private async Task Validations(UserInput input)
    {
        (User? checkUserByUserName, string _) = await _getUserByUserNameOrEmail.Execute(input.UserName);

        if (checkUserByUserName is not null)
        {
            throw new Exception("Já existe um usuário com esse nome de usuário");
        }

        (User? checkUserByEmail, string _) = await _getUserByUserNameOrEmail.Execute(input.Email);

        if (checkUserByEmail is not null)
        {
            throw new Exception("Já existe um usuário com esse e-mail");
        }
    }

    private async Task<User> SaveUser(UserInput input)
    {
        DateTime date = GetDate();

        User user = new()
        {
            FullName = input.FullName,
            Email = input.Email,
            Password = EncryptPassword(input.Password),
            ChangePasswordCode = GetRandomString(22, false),
            ChangePasswordCodeValidity = date.AddDays(7),
            Status = true,
            Date = date
        };

        await _context.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }

    private async Task SaveUserRole(UserInput input, Guid userId)
    {
        UserRole userRole = new()
        {
            UserId = userId,
            Role = input.UserRole,
            CreatedDate = GetDate()
        };

        await _context.AddAsync(userRole);
        await _context.SaveChangesAsync();
    }
}