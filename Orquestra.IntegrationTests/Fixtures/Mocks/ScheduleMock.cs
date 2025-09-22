using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Fixtures.Mocks;

public static class ScheduleMock
{
    public static Schedule Create(Guid clientId, Guid companyId)
    {
        var input = new Schedule
        {
            ScheduleId = Guid.NewGuid(),
            Date = GetDate().AddDays(GetRandomNumber(1, 7)),
            DurationMinutes = GetRandomNumber(1, 5),
            PaymentType = PaymentTypeEnum.Credito,
            ScheduleStatus = ScheduleStatusEnum.Scheduled,
            ClientId = clientId,
            CompanyId = companyId,
            CustomTitle = GenerateTrueOrFalse() ? GetRandomString(charLength: GetRandomNumber(10, 100), onlyUpper: false) : string.Empty,
            CustomUrl = $"https://{GetRandomString(charLength: 22, onlyUpper: false)}",
            Observation = GetRandomString(charLength: 100, onlyUpper: false)
        };

        return input;
    }

    public static List<Schedule> CreateList(int amount, Client client, Company company)
    {
        List<Schedule> list = [];

        for (int i = 0; i < amount; i++)
        {
            list.Add(Create(client.ClientId, company.CompanyId));
        }

        return list;
    }
    public static IEnumerable<object[]> GetUsersClientsCompanies()
    {
        for (int i = 0; i < 24; i++)
        {
            User user = UserMock.Create($"User{i} weon", $"user{i}@test.com", UserRoleEnum.Common);
            Client client = ClientMock.Create();

            yield return new object[] { user, client, client.Company ?? new Company() };
        }
    }
}