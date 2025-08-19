using Orquestra.Domain.Entities;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.UnitTests.Fixtures.Mocks;

public static class ClientMock
{
    public static Client Create()
    {
        var input = new Client
        {
            ClientId = Guid.NewGuid(),
            FullName = GetRandomString(GetRandomNumber(5, 15), false),
            Email = $"{GetRandomString(GetRandomNumber(5, 15), false)}@gmail.com",
            CPF = GetRandomString(11, false),
            Address = GetRandomString(GetRandomNumber(5, 30), false),
            DateOfBirth = GetDate().AddYears(-(GetDate().Year - 1997)),
            CompanyId = Guid.NewGuid()
        };

        return input;
    }

    public static List<Client> CreateList(int j)
    {
        List<Client> list = [];

        for (int i = 0; i < j; i++)
        {
            list.Add(Create());
        }

        return list;
    }
}