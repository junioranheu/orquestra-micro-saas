using AutoMapper;
using Moq;
using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.CompanyUsers.Get;
using Orquestra.Application.UseCases.Schedules.Create;
using Orquestra.Application.UseCases.Schedules.Get;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Domain.Entities;
using Orquestra.UnitTests.Fixtures;
using Orquestra.UnitTests.Fixtures.Mocks;

namespace Orquestra.UnitTests.Tests;

public sealed class ScheduleTest
{
    private readonly IMapper _map;
    private readonly IGetCompanyUser _getCompanyUser;
    private readonly IGetClient _getClient;
    private readonly IGetCompany _getCompany;

    public ScheduleTest()
    {
        _map = Fixture.CreateMapper();

        _getCompanyUser = new Mock<IGetCompanyUser>().Object;
        _getClient = new Mock<IGetClient>().Object;
        _getCompany = new Mock<IGetCompany>().Object;
    }

    [Fact]
    public async Task Execute_ShouldCreateAndReturn_ScheduleOutput()
    {
        // Arrange
        using var context = Fixture.CreateContext();
        var service = new CreateSchedule(context, _map, _getCompanyUser, _getClient, _getCompany);

        var userId = Guid.NewGuid();
        var input = ScheduleMock.Create();

        // Act
        ScheduleOutput output = await service.Execute(userId, input);
        Schedule? savedSchedule = await context.Schedules.FindAsync(output.ScheduleId);

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

    [Fact]
    public async Task Execute_ShouldReturn_ScheduleOutput_WhenScheduleExists()
    {
        // Arrange
        using var context = Fixture.CreateContext();
        var inputList = ScheduleMock.CreateList();

        foreach (var item in inputList)
        {
            Schedule output = _map.Map<Schedule>(item);

            output.Clients = new Client();
            output.Companies = new Company();

            await Fixture.Save(context, output);
        }

        Guid? id = inputList is not null ? inputList.FirstOrDefault()?.ScheduleId : Guid.NewGuid();

        var service = new GetSchedule(context, _map);

        // Act
        ScheduleOutput? result = await service.Execute(id.GetValueOrDefault());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.ScheduleId);
    }
}