using Mapster;
using Moq;
using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Schedules.Base;
using Orquestra.Application.UseCases.Schedules.Create;
using Orquestra.Application.UseCases.Schedules.Get;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Domain.Entities;
using Orquestra.UnitTests.Fixtures;
using Orquestra.UnitTests.Fixtures.Mocks;

namespace Orquestra.UnitTests.Tests;

public sealed class ScheduleTest
{
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser;
    private IGetClient _getClient;
    private IGetCompany _getCompany;

    public ScheduleTest()
    {
        _checkIfUserIsLinkedCompanyUser = new Mock<ICheckIfUserIsLinkedCompanyUser>().Object;
        _getClient = new Mock<IGetClient>().Object;
        _getCompany = new Mock<IGetCompany>().Object;
    }

    [Fact]
    public async Task Execute_ShouldCreateAndReturn_ScheduleOutput()
    {
        // Arrange
        using var context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Client client = ClientMock.Create();
        await Fixture.Save(context, client);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        Schedule input = ScheduleMock.Create(client.ClientId, company.CompanyId);
        var inputConvert = input.Adapt<ScheduleInput>();

        _getClient = new GetClient(context, _checkIfUserIsLinkedCompanyUser);
        _getCompany = new GetCompany(context, _checkIfUserIsLinkedCompanyUser);

        ScheduleBaseDependencies deps = new(context, _checkIfUserIsLinkedCompanyUser, _getClient, _getCompany);
        var service = new CreateSchedule(deps);

        // Act
        ScheduleOutput output = await service.Execute(user.UserId, inputConvert);
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

        Client client = ClientMock.Create();
        await Fixture.Save(context, client);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        List<Schedule>? inputList = ScheduleMock.CreateList(amount: 10, client, company);

        foreach (var item in inputList)
        {
            var output = item.Adapt<Schedule>();
            await Fixture.Save(context, output);
        }

        ScheduleBaseDependencies deps = new(context, _checkIfUserIsLinkedCompanyUser, _getClient, _getCompany);
        var service = new GetSchedule(deps);

        Guid userId = Guid.NewGuid();
        Guid? scheduleId = inputList is not null ? inputList.FirstOrDefault()?.ScheduleId : Guid.NewGuid();

        // Act
        ScheduleOutput? result = await service.Execute(userId: userId, scheduleId: scheduleId.GetValueOrDefault());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(scheduleId, result.ScheduleId);
    }
}