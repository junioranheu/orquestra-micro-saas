using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Fixtures.Mocks;

public static class CompanyMock
{
    public static Company Create()
    {
        return new Company
        {
            CompanyId = Guid.NewGuid(),
            Name = GetRandomString(charLength: GetRandomNumber(5, 15)),
            Email = $"{GetRandomString(charLength: GetRandomNumber(5, 15))}@gmail.com",
            Phone = $"12 9827163{GetRandomNumber(2, 2)}",
            CompanyType = CompanyTypeEnum.ClinicaOdontologia,
            Address = GetRandomString(charLength: GetRandomNumber(5, 15)),
            AddressNumber = "480",
            City = GetRandomString(charLength: GetRandomNumber(5, 15)),
            State = GetRandomString(charLength: GetRandomNumber(5, 15)),
            ZipCode = "12605110",
            Country = "Brasil",
            Logo = [137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82, 0, 0, 0, 1, 0, 0, 0, 1, 8, 6, 0, 0, 0, 31, 21, 196, 137, 0, 0, 0, 12, 73, 68, 65, 84, 8, 153, 99, 96, 0, 0, 0, 2, 0, 1, 226, 33, 189, 167, 0, 0, 0, 0, 73, 69, 78, 68, 174, 66, 96, 130],
            LogoContentType = "image/png",
            Color = "#FFFFFF",
            CompanySituation = CompanySituationEnum.Approved,
            PlanStartDate = GetDate(),
            PlanEndDate = GetDate().AddDays(7),
            Status = true
        };
    }

    public static List<Company> CreateList(int amount)
    {
        List<Company> list = [];

        for (int i = 0; i < amount; i++)
        {
            list.Add(Create());
        }

        return list;
    }
}