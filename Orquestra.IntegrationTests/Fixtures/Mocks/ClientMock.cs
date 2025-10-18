using Orquestra.Domain.Entities;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Fixtures.Mocks;

public static class ClientMock
{
    public static Client Create()
    {
        Company company = CompanyMock.Create();

        var input = new Client 
        {
            ClientId = Guid.NewGuid(),
            FullName = $"{GetRandomString(charLength: GetRandomNumber(5, 15), onlyLetters: true)} {GetRandomString(charLength: GetRandomNumber(5, 15), onlyLetters: true)}",
            Email = $"{GetRandomString(charLength: GetRandomNumber(5, 15))}@gmail.com",
            CPF = "858.136.140-44",
            Address = GetRandomString(charLength: GetRandomNumber(5, 15)),
            AddressNumber = "480",
            City = GetRandomString(charLength: GetRandomNumber(5, 15)),
            State = GetRandomString(charLength: GetRandomNumber(5, 15)),
            ZipCode = "12605110",
            Country = "Brasil",
            DateOfBirth = GetDate().AddYears(-(GetDate().Year - 1997)),
            Phone = $"12 9827163{GetRandomNumber(2, 2)}",
            CompanyId = company.CompanyId,
            Company = company
        };

        return input;
    }

    public static List<Client> CreateList(int amount)
    {
        List<Client> list = [];

        for (int i = 0; i < amount; i++)
        {
            list.Add(Create());
        }

        return list;
    }
}