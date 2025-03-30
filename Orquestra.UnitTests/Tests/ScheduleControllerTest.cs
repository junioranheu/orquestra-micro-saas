using Microsoft.AspNetCore.Mvc;
using Moq;
using Orquestra.API.Controllers;
using Orquestra.Application.UseCases.Schedules.Create;
using Orquestra.Application.UseCases.Schedules.Get;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Domain.Enums;
using Orquestra.UnitTests.Fixtures;

namespace Orquestra.UnitTests.Tests;

public sealed class ScheduleControllerTest
{
    private readonly Mock<IGetSchedule> _getMock;
    private readonly Mock<ICreateSchedule> _createMock;
    private readonly ScheduleController _controller;

    public ScheduleControllerTest()
    {
        _getMock = new Mock<IGetSchedule>();
        _createMock = new Mock<ICreateSchedule>();

        _controller = new ScheduleController(_getMock.Object, _createMock.Object);
    }

    [Fact]
    public async Task Create_ShouldReturn_ScheduleOutput()
    {
        // Arrange
        using var context = Fixture.CreateContext();
        Fixture.FakeAuth(_controller);

        var input = new ScheduleInput
        {
            ScheduleId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            PaymentType = PaymentTypeEnum.Credito,
            ScheduleStatus = ScheduleStatusEnum.Scheduled,
            ClientId = Guid.NewGuid(),
            CompanyId = Guid.NewGuid()
        };

        var expectedOutput = new ScheduleOutput
        {
            ScheduleId = input.ScheduleId,
            Date = input.Date,
            PaymentType = input.PaymentType,
            ScheduleStatus = input.ScheduleStatus,
            ClientId = input.ClientId,
            CompanyId = input.CompanyId
        };

        _createMock.
            Setup(x => x.Execute(It.IsAny<Guid>(), input)).
            ReturnsAsync(expectedOutput);

        // Act;
        var result = await _controller.Create(input);

        // Assert;
        var actionResult = Assert.IsType<ActionResult<ScheduleOutput>>(result);
        var actualOutput = Assert.IsType<ScheduleOutput>(actionResult.Value);

        Assert.Equal(expectedOutput, actualOutput);
    }
}