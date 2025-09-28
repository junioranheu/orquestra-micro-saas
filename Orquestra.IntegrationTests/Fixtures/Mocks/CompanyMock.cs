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
            City = GetRandomString(charLength: GetRandomNumber(5, 15)),
            State = GetRandomString(charLength: GetRandomNumber(5, 15)),
            ZipCode = string.Empty,
            Country = "Brasil",
            LogoUrl = "https://placehold.co/200x200",
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