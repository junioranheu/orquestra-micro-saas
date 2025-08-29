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
            Name = GetRandomString(GetRandomNumber(5, 15), false),
            Email = $"{GetRandomString(GetRandomNumber(5, 15), false)}@gmail.com",
            Phone = GetRandomString(GetRandomNumber(5, 15), false),
            CompanyType = CompanyTypeEnum.ClinicaOdontologia,
            Address = GetRandomString(GetRandomNumber(5, 15), false),
            City = GetRandomString(GetRandomNumber(5, 15), false),
            State = GetRandomString(GetRandomNumber(5, 15), false),
            ZipCode = string.Empty,
            Country = "Brasil",
            LogoUrl = "https://placehold.co/200x200",
            Color = "#FFFFFF",
            CompanySituation = CompanySituationEnum.Approved,
            PlanType = PlanTypeEnum.Basic,
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