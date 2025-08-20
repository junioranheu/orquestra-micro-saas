using Mapster;
using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Schedules.Base;
using Orquestra.Application.UseCases.Schedules.Create;
using Orquestra.Application.UseCases.Schedules.Get;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using Orquestra.UnitTests.Fixtures;
using Orquestra.UnitTests.Fixtures.Mocks;

namespace Orquestra.UnitTests.Tests;

public sealed class ScheduleTest
{
    private IGetCompanyUserByCompanyId? _getCompanyUserByCompanyId;
    private ICheckIfUserIsLinkedCompanyUser? _checkIfUserIsLinkedCompanyUser;
    private IGetClient? _getClient;
    private IGetCompany? _getCompany;

    private static async Task<User> CreateUser(Context context)
    {
        var user = UserMock.Create();
        await Fixture.Save(context, user);

        return user;
    }

    private static async Task<(Client client, Company company)> CreateClientAndCompany(Context context)
    {
        var client = ClientMock.Create();
        var company = CompanyMock.Create();

        await Fixture.Save(context, client);
        await Fixture.Save(context, company);

        return (client, company);
    }

    #region tests
    [Fact]
    public async Task Execute_ShouldCreateSchedule_WhenInputIsValid()
    {
        // Arrange
        using var context = Fixture.CreateContext();

        var user = await CreateUser(context);
        var (client, company) = await CreateClientAndCompany(context);

        var scheduleInput = ScheduleMock.Create(client.ClientId, company.CompanyId).Adapt<ScheduleInput>();

        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        _getCompanyUserByCompanyId = new GetCompanyUserByCompanyId(context);
        _checkIfUserIsLinkedCompanyUser = new CheckIfUserIsLinkedCompanyUser(_getCompanyUserByCompanyId, httpContextAccessor);
        _getClient = new GetClient(context, _checkIfUserIsLinkedCompanyUser);
        _getCompany = new GetCompany(context, _checkIfUserIsLinkedCompanyUser);

        var deps = new ScheduleBaseDependencies(context, _checkIfUserIsLinkedCompanyUser, _getClient, _getCompany);
        var service = new CreateSchedule(deps);

        // Act
        var output = await service.Execute(user.UserId, scheduleInput);
        var savedSchedule = await context.Schedules.FindAsync(output.ScheduleId);

        // Assert
        Assert.NotNull(output);
        Assert.Equal(scheduleInput.ClientId, output.ClientId);
        Assert.Equal(scheduleInput.CompanyId, output.CompanyId);
        Assert.Equal(scheduleInput.Date, output.Date);

        Assert.NotNull(savedSchedule);
        Assert.Equal(output.ScheduleId, savedSchedule.ScheduleId);
    }

    [Fact]
    public async Task Execute_ShouldReturnExistingSchedule_WhenScheduleExists()
    {
        // Arrange
        using var context = Fixture.CreateContext();

        var (client, company) = await CreateClientAndCompany(context);
        var schedules = ScheduleMock.CreateList(10, client, company);

        foreach (var schedule in schedules)
        {
            await Fixture.Save(context, schedule);
        }

        var user = await CreateUser(context);
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        _getCompanyUserByCompanyId = new GetCompanyUserByCompanyId(context);
        _checkIfUserIsLinkedCompanyUser = new CheckIfUserIsLinkedCompanyUser(_getCompanyUserByCompanyId, httpContextAccessor);
        _getClient = new GetClient(context, _checkIfUserIsLinkedCompanyUser);
        _getCompany = new GetCompany(context, _checkIfUserIsLinkedCompanyUser);

        var deps = new ScheduleBaseDependencies(context, _checkIfUserIsLinkedCompanyUser, _getClient, _getCompany);
        var service = new GetSchedule(deps);

        var userId = Guid.NewGuid();
        var scheduleId = schedules.First().ScheduleId;

        // Act
        var result = await service.Execute(userId, scheduleId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(scheduleId, result.ScheduleId);
        Assert.Equal(client.ClientId, result.ClientId);
        Assert.Equal(company.CompanyId, result.CompanyId);
    }
    #endregion
}