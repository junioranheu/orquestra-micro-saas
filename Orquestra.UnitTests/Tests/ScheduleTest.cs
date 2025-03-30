using AutoMapper;
using Moq;
using Orquestra.Application.UseCases.CompanyUsers.Get;
using Orquestra.Application.UseCases.Schedules.Create;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Domain.Enums;
using Orquestra.UnitTests.Fixtures;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.UnitTests.Tests;

public sealed class ScheduleTest
{
    private readonly IMapper _mapper;
    private readonly IGetCompanyUser _getCompanyUser;

    public ScheduleTest()
    {
        _mapper = Fixture.CreateMapper();

        var getCompanyUserMock = new Mock<IGetCompanyUser>();
        _getCompanyUser = getCompanyUserMock.Object;
    }

    [Fact]
    public async Task Execute_ShouldCreateAndReturn_ScheduleOutput()
    {
        // Arrange
        using var context = Fixture.CreateContext(); 
        var service = new CreateSchedule(context, _mapper, _getCompanyUser);

        var userId = Guid.NewGuid();

        var input = new ScheduleInput
        {
            ScheduleId = Guid.NewGuid(),
            Date = GetDate().AddDays(GetRandomNumber(1, 7)),
            PaymentType = PaymentTypeEnum.Credito,
            ScheduleStatus = ScheduleStatusEnum.Scheduled,
            ClientId = Guid.NewGuid(),
            CompanyId = Guid.NewGuid()
        };

        // Act
        var output = await service.Execute(userId, input);
        var savedSchedule = await context.Schedules.FindAsync(output.ScheduleId);

        // Assert
        Assert.NotNull(output);
        Assert.Equal(input.ScheduleId, output.ScheduleId);
        Assert.Equal(input.Date, output.Date);
        Assert.Equal(input.PaymentType, output.PaymentType);
        Assert.Equal(input.ScheduleStatus, output.ScheduleStatus);
        Assert.Equal(input.ClientId, output.ClientId);
        Assert.Equal(input.CompanyId, output.CompanyId);

        Assert.NotNull(savedSchedule);
        Assert.Equal(input.ScheduleId, savedSchedule.ScheduleId);
    }
}