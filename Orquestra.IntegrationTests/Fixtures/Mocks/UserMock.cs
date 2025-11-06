using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using static Orquestra.Utils.Fixtures.Encrypt;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Fixtures.Mocks;

public static class UserMock
{
    public static User Create()
    {
        User input = new()
        {
            UserId = Guid.NewGuid(),
            FullName = $"{GetRandomString(charLength: GetRandomNumber(5, 15), onlyLetters: true)} {GetRandomString(charLength: GetRandomNumber(5, 15), onlyLetters: true)}",
            Email = $"{GetRandomString(charLength: GetRandomNumber(5, 15))}@gmail.com",
            Password = EncryptPassword(GetRandomString(charLength: 11)),
            Role = UserRoleEnum.Common,
            RecoverPasswordQuestion = RecoverPasswordQuestionEnum.MotherName,
            RecoverPasswordAnswer = "Sandra"
        };

        return input;
    }

    public static User Create(string fullName, string email, UserRoleEnum role)
    {
        return new User
        {
            UserId = Guid.NewGuid(),
            FullName = fullName,
            Email = email,
            Password = EncryptPassword(GetRandomString(charLength: 11)),
            Role = role,
            RecoverPasswordQuestion = RecoverPasswordQuestionEnum.MotherName,
            RecoverPasswordAnswer = "Sandra"
        };
    }

    public static User Create(string fullName, string email, string password, UserRoleEnum role)
    {
        return new User
        {
            UserId = Guid.NewGuid(),
            FullName = fullName,
            Email = email,
            Password = EncryptPassword(password),
            Role = role,
            RecoverPasswordQuestion = RecoverPasswordQuestionEnum.MotherName,
            RecoverPasswordAnswer = "Sandra"
        };
    }

    public static List<User> CreateList(int amount)
    {
        List<User> list = [];

        for (int i = 0; i < amount; i++)
        {
            list.Add(Create());
        }

        return list;
    }
}