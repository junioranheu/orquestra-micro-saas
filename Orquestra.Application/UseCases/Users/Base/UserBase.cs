using Orquestra.Application.UseCases.Users.GetByEmail;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Entities;

namespace Orquestra.Application.UseCases.Users.Base;

public class UserBase(IGetUserByEmail getUserByEmail)
{
    private readonly IGetUserByEmail _getUserByEmail = getUserByEmail;

    public async Task Validate(UserInput input)
    {
        (User? checkUserByEmail, string _) = await _getUserByEmail.Execute(input.Email);

        if (checkUserByEmail is not null)
        {
            throw new Exception("Já existe um usuário com esse e-mail");
        }
    }
}