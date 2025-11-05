using Microsoft.AspNetCore.Http;
using Moq;
using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Schedules.Base;
using Orquestra.Application.UseCases.Schedules.GetAllByClientId;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Messaging.Publishers;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Schedules;

public sealed class GetAllByClientIdTests
{
    [Fact]
    public async Task Execute_ShouldReturnSchedules_WhenClientHasSchedulesAndUserLinked()
    {
        // Arrange;
        (Context context, User user, Company company, Client client, List<Schedule> schedules) = await ArrangeSchedulesWithClientAsync(count: 3);

        GetScheduleByClientId sut = CreateSut(context, user);

        // Act;
        List<ScheduleOutput>? result = await sut.Execute(user.UserId, company.CompanyId, client.ClientId);

        // Assert;
        Assert.NotNull(result);
        Assert.Equal(schedules.Count, result.Count);
        Assert.All(result, r =>
        {
            Assert.Equal(company.CompanyId, r.CompanyId);
            Assert.Equal(client.ClientId, r.ClientId);
        });
    }

    [Fact]
    public async Task Execute_ShouldReturnEmptyList_WhenClientHasNoSchedules()
    {
        // Arrange;
        (Context context, User user, Company company, Client client, _) = await ArrangeSchedulesWithClientAsync(count: 0);

        GetScheduleByClientId sut = CreateSut(context, user);

        // Act;
        List<ScheduleOutput>? result = await sut.Execute(user.UserId, company.CompanyId, client.ClientId);

        // Assert;
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserNotLinkedToCompany()
    {
        // Arrange;
        (Context context, User user, Company company, Client client, List<Schedule> schedules) = await ArrangeSchedulesWithClientAsync(count: 1);

        // Remove vínculos;
        context.CompanyUsers.RemoveRange(context.CompanyUsers);
        await context.SaveChangesAsync();

        // Cria outro user e vincula;
        User user2 = UserMock.Create();
        await Fixture.Save(context, user2);

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user2.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        GetScheduleByClientId sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>    sut.Execute(user.UserId, company.CompanyId, client.ClientId));
    }

    [Fact]
    public async Task Execute_ShouldIgnoreSchedulesFromOtherClientsOrCompanies()
    {
        // Arrange;
        (Context context, User user, Company company, Client client, _) = await ArrangeSchedulesWithClientAsync(count: 0);

        // Cria outro client;
        Client client2 = ClientMock.Create();
        await Fixture.Save(context, client2);

        // Cria um schedule pro client 2;
        Schedule scheduleOtherClient = ScheduleMock.Create(client2.ClientId, company.CompanyId);
        scheduleOtherClient.Status = true;
        await Fixture.Save(context, scheduleOtherClient);

        // Cria outro company;
        Company company2 = CompanyMock.Create();
        company2.Status = true;
        await Fixture.Save(context, company2);

        // Cria schedule em outra empresa
        Schedule scheduleOtherCompany = ScheduleMock.Create(client.ClientId, company2.CompanyId);
        scheduleOtherCompany.Status = true;
        await Fixture.Save(context, scheduleOtherCompany);

        GetScheduleByClientId sut = CreateSut(context, user);

        // Act;
        List<ScheduleOutput>? result = await sut.Execute(user.UserId, company.CompanyId, client.ClientId);

        // Assert;
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #region helpers
    private static async Task<(Context context, User user, Company company, Client client, List<Schedule> schedules)> ArrangeSchedulesWithClientAsync(int count = 1)
    {
        Context context = Fixture.CreateContext();

        // Cria user;
        User user = UserMock.Create();
        await Fixture.Save(context, user);

        // Cria company;
        Company company = CompanyMock.Create();
        company.Status = true;
        await Fixture.Save(context, company);

        // Cria client;
        Client client = ClientMock.Create();
        await Fixture.Save(context, client);

        // Vincula user à empresa;
        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        // Cria schedules;
        List<Schedule> schedules = [];

        for (int i = 0; i < count; i++)
        {
            Schedule schedule = ScheduleMock.Create(client.ClientId, company.CompanyId);
            schedule.Status = true;
            await Fixture.Save(context, schedule);
            schedules.Add(schedule);
        }

        return (context, user, company, client, schedules);
    }

    private static GetScheduleByClientId CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);
        GetClient getClient = new(context, checkIfUserIsLinkedCompanyUser);
        GetCompany getCompany = new(context, checkIfUserIsLinkedCompanyUser);
        Mock<IGenericPublisher> genericPublisherMock = Fixture.CreateGenericPublisher();

        GetScheduleByClientId getScheduleByClientId = new(new ScheduleBaseDependencies(
            context,
            checkIfUserIsLinkedCompanyUser,
            getClient,
            getCompany,
            genericPublisherMock.Object
        ));

        return getScheduleByClientId;
    }
    #endregion
}