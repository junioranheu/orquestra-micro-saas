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
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using System.Diagnostics.CodeAnalysis;

namespace Orquestra.IntegrationTests.Tests;

public sealed class ScheduleTest
{
    [Fact]
    public async Task Execute_ShouldCreateSchedule_WhenInputIsValid()
    {
        // Arrange
        using Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Client client = ClientMock.Create();
        await Fixture.Save(context, client);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        ScheduleInput scheduleInput = ScheduleMock.Create(client.ClientId, company.CompanyId).Adapt<ScheduleInput>();

        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);
        GetClient getClient = new(context, checkIfUserIsLinkedCompanyUser);
        GetCompany getCompany = new(context, checkIfUserIsLinkedCompanyUser);

        ScheduleBaseDependencies deps = new(context, checkIfUserIsLinkedCompanyUser, getClient, getCompany);
        CreateSchedule service = new(deps);

        // Act
        ScheduleOutput output = await service.Execute(user.UserId, scheduleInput);
        Schedule? savedSchedule = await context.Schedules.FindAsync(output.ScheduleId);

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
        using Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Client client = ClientMock.Create();
        await Fixture.Save(context, client);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        var schedules = ScheduleMock.CreateList(10, client, company);

        foreach (var schedule in schedules)
        {
            await Fixture.Save(context, schedule);
        }

        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);
        GetClient getClient = new(context, checkIfUserIsLinkedCompanyUser);
        GetCompany getCompany = new(context, checkIfUserIsLinkedCompanyUser);

        ScheduleBaseDependencies deps = new(context, checkIfUserIsLinkedCompanyUser, getClient, getCompany);
        GetSchedule service = new(deps);

        Guid userId = Guid.NewGuid();
        Guid scheduleId = schedules.First().ScheduleId;

        // Act
        var result = await service.Execute(userId, scheduleId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(scheduleId, result.ScheduleId);
        Assert.Equal(client.ClientId, result.ClientId);
        Assert.Equal(company.CompanyId, result.CompanyId);
    }

    [Theory]
    [MemberData(nameof(ScheduleMock.GetUsersClientsCompanies), MemberType = typeof(ScheduleMock))]
    [SuppressMessage("Usage", "xUnit1042:The member referenced by the MemberData attribute returns untyped data rows", Justification = "<Pendente>")]
    [SuppressMessage("CodeQuality", "IDE0079:Remover a supressão desnecessária", Justification = "<Pendente>")]
    public async Task Execute_ShouldCreateScheduleForDifferentUsers(User user, Client client, Company company)
    {
        // Arrange
        using Context context = Fixture.CreateContext();

        await Fixture.Save(context, user);
        await Fixture.Save(context, client); // Por algum motivo, esse client já salva automaticamente o company também;

        ScheduleInput scheduleInput = ScheduleMock.Create(client.ClientId, company.CompanyId).Adapt<ScheduleInput>();

        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);
        GetClient getClient = new(context, checkIfUserIsLinkedCompanyUser);
        GetCompany getCompany = new(context, checkIfUserIsLinkedCompanyUser);

        ScheduleBaseDependencies deps = new(context, checkIfUserIsLinkedCompanyUser, getClient, getCompany);
        CreateSchedule service = new(deps);

        // Act
        ScheduleOutput output = await service.Execute(user.UserId, scheduleInput);
        Schedule? savedSchedule = await context.Schedules.FindAsync(output.ScheduleId);

        // Assert
        Assert.NotNull(output);
        Assert.Equal(scheduleInput.ClientId, output.ClientId);
        Assert.Equal(scheduleInput.CompanyId, output.CompanyId);
        Assert.Equal(scheduleInput.Date, output.Date);
        Assert.NotNull(savedSchedule);
        Assert.Equal(output.ScheduleId, savedSchedule.ScheduleId);
    }
}