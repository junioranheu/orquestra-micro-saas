using AutoMapper;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.UnitTests.Fixtures.Mocks;

public static class ScheduleMock
{
    public static ScheduleInput Create(Guid clientId, Guid companyId)
    {
        var input = new ScheduleInput
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

    public static List<ScheduleInput> CreateList(int j, Guid clientId, Guid companyId)
    {
        List<ScheduleInput> list = [];

        for (int i = 0; i < j; i++)
        {
            list.Add(Create(clientId, companyId));
        }

        return list;
    }

    public static Schedule Create(IMapper _map, Guid clientId, Guid companyId)
    {
        Schedule output = _map.Map<Schedule>(Create(clientId, companyId));
        return output;
    }

    public static List<Schedule> CreateList(IMapper _map, int j, Guid clientId, Guid companyId)
    {
        List<Schedule> output = _map.Map<List<Schedule>>(CreateList(j, clientId, companyId));
        return output;
    }
}