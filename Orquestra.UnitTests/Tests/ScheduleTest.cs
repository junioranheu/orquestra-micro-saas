using AutoMapper;
using Moq;
using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
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
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser;
    private readonly IGetClient _getClient;
    private readonly IGetCompany _getCompany;

    public ScheduleTest()
    {
        _map = Fixture.CreateMapper();

        _checkIfUserIsLinkedCompanyUser = new Mock<ICheckIfUserIsLinkedCompanyUser>().Object;
        _getClient = new Mock<IGetClient>().Object;
        _getCompany = new Mock<IGetCompany>().Object;
    }

    [Fact]
    public async Task Execute_ShouldCreateAndReturn_ScheduleOutput()
    {
        // Arrange
        using var context = Fixture.CreateContext();
        var service = new CreateSchedule(context, _map, _checkIfUserIsLinkedCompanyUser, _getClient, _getCompany);

        var userId = Guid.NewGuid();

        Client client = ClientMock.Create(_map);
        await Fixture.Save(context, client);

        Company company = CompanyMock.Create(_map);
        await Fixture.Save(context, company);

        ScheduleInput input = ScheduleMock.Create(client.ClientId, company.CompanyId);

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

        Client client = ClientMock.Create(_map);
        await Fixture.Save(context, client);

        Company company = CompanyMock.Create(_map);
        await Fixture.Save(context, company);

        List<Schedule>? inputList = ScheduleMock.CreateList(_map, j: 10, client, company);

        foreach (var item in inputList)
        {
            Schedule output = _map.Map<Schedule>(item);
            await Fixture.Save(context, output);
        }

        var service = new GetSchedule(context, _map);
        Guid? id = inputList is not null ? inputList.FirstOrDefault()?.ScheduleId : Guid.NewGuid();

        // Act
        ScheduleOutput? result = await service.Execute(id.GetValueOrDefault());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.ScheduleId);
    }
}