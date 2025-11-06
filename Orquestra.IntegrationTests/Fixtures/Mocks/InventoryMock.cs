using Orquestra.Domain.Entities;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Fixtures.Mocks;

public static class InventoryMock
{
    public static Inventory Create()
    {
        Company company = CompanyMock.Create();

        var input = new Inventory
        {
            InventoryId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            Company = company,
            Name = GetRandomString(charLength: GetRandomNumber(5, 20), onlyLetters: true),
            Description = GetRandomString(charLength: GetRandomNumber(10, 50)),
            Quantity = GetRandomNumber(1, 200),
            UnitPrice = Math.Round((decimal)GetRandomNumber(10, 5000) / 1.37m, 2),
            Image = null,
            ImageContentType = null
        };

        return input;
    }

    public static List<Inventory> CreateList(int amount)
    {
        List<Inventory> list = [];

        for (int i = 0; i < amount; i++)
        {
            list.Add(Create());
        }

        return list;
    }
}