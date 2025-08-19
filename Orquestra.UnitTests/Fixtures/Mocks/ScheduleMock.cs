using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.UnitTests.Fixtures.Mocks;

public static class ScheduleMock
{
    public static Schedule Create(Guid clientId, Guid companyId)
    {
        var input = new Schedule
        {
            ScheduleId = Guid.NewGuid(),
            Date = GetDate().AddDays(GetRandomNumber(1, 7)),
            PaymentType = PaymentTypeEnum.Credito,
            ScheduleStatus = ScheduleStatusEnum.Scheduled,
            ClientId = clientId,
            CompanyId = companyId
        };

        return input;
    }

    public static List<Schedule> CreateList(int j, Client client, Company company)
    {
        List<Schedule> list = [];

        for (int i = 0; i < j; i++)
        {
            list.Add(Create(client.ClientId, company.CompanyId));
        }

        return list;
    }
}