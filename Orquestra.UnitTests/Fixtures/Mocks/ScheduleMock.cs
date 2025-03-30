using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Domain.Enums;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.UnitTests.Fixtures.Mocks;

public static class ScheduleMock
{
    public static ScheduleInput Create()
    {
        var input = new ScheduleInput
        {
            ScheduleId = Guid.NewGuid(),
            Date = GetDate().AddDays(GetRandomNumber(1, 7)),
            PaymentType = PaymentTypeEnum.Credito,
            ScheduleStatus = ScheduleStatusEnum.Scheduled,
            ClientId = Guid.NewGuid(),
            CompanyId = Guid.NewGuid()
        };

        return input;
    }

    public static List<ScheduleInput> CreateList()
    {
        List<ScheduleInput> list =
        [
            Create(),
            Create()
        ];

        return list;
    }
}