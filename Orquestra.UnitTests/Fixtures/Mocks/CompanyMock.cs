using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.UnitTests.Fixtures.Mocks;

public static class CompanyMock
{
    public static Company Create()
    {
        var input = new Company
        {
            CompanyId = Guid.NewGuid(),
            Name = GetRandomString(GetRandomNumber(5, 15), false),
            Email = $"{GetRandomString(GetRandomNumber(5, 15), false)}@gmail.com",
            Phone = GetRandomString(GetRandomNumber(5, 15), false),
            Type = CompanyTypeEnum.ClinicaOdontologia
        };

        return input;
    }

    public static List<Company> CreateList(int j)
    {
        List<Company> list = [];

        for (int i = 0; i < j; i++)
        {
            list.Add(Create());
        }

        return list;
    }
}