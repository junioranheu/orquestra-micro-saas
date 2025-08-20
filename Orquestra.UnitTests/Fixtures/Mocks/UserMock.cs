using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using static Orquestra.Utils.Fixtures.Encrypt;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.UnitTests.Fixtures.Mocks;

public static class UserMock
{
    public static User Create()
    {
        var input = new User
        {
            UserId = Guid.NewGuid(),
            FullName = GetRandomString(GetRandomNumber(5, 15), false),
            Email = $"{GetRandomString(GetRandomNumber(5, 15), false)}@gmail.com",
            Password = EncryptPassword(GetRandomString(11, false)),
            Role = UserRoleEnum.Common
        };

        return input;
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