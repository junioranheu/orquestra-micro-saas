using Microsoft.AspNetCore.Http;
using Moq;
using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Schedules.Base;
using Orquestra.Application.UseCases.Schedules.GetAllByCompanyId;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Messaging.Publishers;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Schedules;

public sealed class GetAllByCompanyIdTests
{
    [Fact]
    public async Task Execute_ShouldReturnSchedules_WhenSchedulesExistAndUserLinked()
    {
        // Arrange;
        (Context context, User user, Company company, List<Schedule> schedules) = await ArrangeSchedulesWithUserAsync(count: 3);

        GetScheduleByCompanyId sut = CreateSut(context, user);

        // Act;
        List<ScheduleOutput>? result = await sut.Execute(user.UserId, company.CompanyId, null, null);

        // Assert;
        Assert.NotNull(result);
        Assert.Equal(schedules.Count, result.Count);
    }

    [Fact]
    public async Task Execute_ShouldReturnSchedules_WhenYearAndMonthProvided_FiltersByPreviousCurrentAndNextMonth()
    {
        // Arrange;
        (Context context, User user, Company company, _) = await ArrangeSchedulesWithUserAsync(count: 0);

        // Cria um cliente;
        Client client = ClientMock.Create();
        await Fixture.Save(context, client);

        // Datas: fev, mar, abr de 2025;
        Schedule feb = ScheduleMock.Create(client.ClientId, company.CompanyId);
        feb.DateStart = new DateTime(2025, 2, 10, 10, 0, 0);
        feb.Status = true;
        await Fixture.Save(context, feb);

        Schedule mar = ScheduleMock.Create(client.ClientId, company.CompanyId);
        mar.DateStart = new DateTime(2025, 3, 15, 14, 0, 0);
        mar.Status = true;
        await Fixture.Save(context, mar);

        Schedule apr = ScheduleMock.Create(client.ClientId, company.CompanyId);
        apr.DateStart = new DateTime(2025, 4, 20, 9, 0, 0);
        apr.Status = true;
        await Fixture.Save(context, apr);

        // Fora do range: jan/2025 e mai/2025;
        Schedule jan = ScheduleMock.Create(client.ClientId, company.CompanyId);
        jan.DateStart = new DateTime(2025, 1, 5, 8, 0, 0);
        jan.Status = true;
        await Fixture.Save(context, jan);

        Schedule may = ScheduleMock.Create(client.ClientId, company.CompanyId);
        may.DateStart = new DateTime(2025, 5, 1, 11, 0, 0);
        may.Status = true;
        await Fixture.Save(context, may);

        GetScheduleByCompanyId sut = CreateSut(context, user);

        // Act;
        List<ScheduleOutput>? result = await sut.Execute(user.UserId, company.CompanyId, 2025, 3);

        // Assert;
        Assert.NotNull(result);
        Assert.Contains(result, r => r.ScheduleId == feb.ScheduleId);
        Assert.Contains(result, r => r.ScheduleId == mar.ScheduleId);
        Assert.Contains(result, r => r.ScheduleId == apr.ScheduleId);
        Assert.DoesNotContain(result, r => r.ScheduleId == jan.ScheduleId);
        Assert.DoesNotContain(result, r => r.ScheduleId == may.ScheduleId);
    }

    [Fact]
    public async Task Execute_ShouldReturnEmptyList_WhenNoSchedulesExistForCompany()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        company.Status = true;
        await Fixture.Save(context, company);

        GetScheduleByCompanyId sut = CreateSut(context, user);

        // Act;
        List<ScheduleOutput>? result = await sut.Execute(user.UserId, company.CompanyId, null, null);

        // Assert;
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserNotLinkedToCompany()
    {
        // Arrange;
        (Context context, User user, Company company, List<Schedule> schedules) = await ArrangeSchedulesWithUserAsync(count: 1);

        // Remove vínculos;
        context.CompanyUsers.RemoveRange(context.CompanyUsers);
        await context.SaveChangesAsync();

        // Cria outro usuário;
        User user2 = UserMock.Create();
        await Fixture.Save(context, user2);

        // Vincula o user2;
        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = schedules[0].CompanyId,
            UserId = user2.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        GetScheduleByCompanyId sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, company.CompanyId, null, null));
    }

    [Fact]
    public async Task Execute_ShouldReturnEmptyList_WhenYearDoesNotMatchAnySchedule()
    {
        // Arrange;
        (Context context, User user, Company company, _) = await ArrangeSchedulesWithUserAsync(count: 0);

        // Cria um cliente;
        Client client = ClientMock.Create();
        await Fixture.Save(context, client);

        // Cria um schedule em 2024;
        Schedule s = ScheduleMock.Create(client.ClientId, company.CompanyId);
        s.DateStart = new DateTime(2024, 6, 1);
        s.Status = true;
        await Fixture.Save(context, s);

        GetScheduleByCompanyId sut = CreateSut(context, user);

        // Act;
        List<ScheduleOutput>? result = await sut.Execute(user.UserId, company.CompanyId, 1997, null);

        // Assert;
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Execute_ShouldReturnSchedulesFilteredByYear_WhenOnlyYearProvided()
    {
        // Arrange;
        (Context context, User user, Company company, _) = await ArrangeSchedulesWithUserAsync(count: 0);

        Client client = ClientMock.Create();
        await Fixture.Save(context, client);

        Schedule s2025 = ScheduleMock.Create(client.ClientId, company.CompanyId);
        s2025.DateStart = new DateTime(2025, 7, 1);
        s2025.Status = true;
        await Fixture.Save(context, s2025);

        Schedule s2024 = ScheduleMock.Create(client.ClientId, company.CompanyId);
        s2024.DateStart = new DateTime(2024, 12, 1);
        s2024.Status = true;
        await Fixture.Save(context, s2024);

        GetScheduleByCompanyId sut = CreateSut(context, user);

        // Act;
        List<ScheduleOutput>? result = await sut.Execute(user.UserId, company.CompanyId, 2025, null);

        // Assert;
        Assert.NotNull(result);
        Assert.All(result, r => Assert.Equal(2025, r.DateStart.Year));
    }

    [Fact]
    public async Task Execute_ShouldReturnSchedules_WhenGetNearbyDatesIsTrue()
    {
        // Arrange;
        (Context context, User user, Company company, _) = await ArrangeSchedulesWithUserAsync(count: 0);

        Client client = ClientMock.Create();
        await Fixture.Save(context, client);

        DateTime today = GetDate();
        DateTime yesterday = today.Date.AddDays(-1);
        DateTime tomorrow = today.Date.AddDays(1);
        DateTime twoDaysAgo = today.Date.AddDays(-2);
        DateTime twoDaysLater = today.Date.AddDays(2);

        // Dentro do range;
        Schedule schedYesterday = ScheduleMock.Create(client.ClientId, company.CompanyId);
        schedYesterday.DateStart = yesterday.AddHours(10);
        schedYesterday.Status = true;
        await Fixture.Save(context, schedYesterday);

        Schedule schedToday = ScheduleMock.Create(client.ClientId, company.CompanyId);
        schedToday.DateStart = today.AddHours(15);
        schedToday.Status = true;
        await Fixture.Save(context, schedToday);

        Schedule schedTomorrow = ScheduleMock.Create(client.ClientId, company.CompanyId);
        schedTomorrow.DateStart = tomorrow.AddHours(9);
        schedTomorrow.Status = true;
        await Fixture.Save(context, schedTomorrow);

        // Fora do range;
        Schedule schedTwoDaysAgo = ScheduleMock.Create(client.ClientId, company.CompanyId);
        schedTwoDaysAgo.DateStart = twoDaysAgo.AddHours(8);
        schedTwoDaysAgo.Status = true;
        await Fixture.Save(context, schedTwoDaysAgo);

        Schedule schedTwoDaysLater = ScheduleMock.Create(client.ClientId, company.CompanyId);
        schedTwoDaysLater.DateStart = twoDaysLater.AddHours(11);
        schedTwoDaysLater.Status = true;
        await Fixture.Save(context, schedTwoDaysLater);

        GetScheduleByCompanyId sut = CreateSut(context, user);

        // Act;
        List<ScheduleOutput>? result = await sut.Execute(user.UserId, company.CompanyId, null, null, true);

        // Assert;
        Assert.NotNull(result);
        Assert.Contains(result, r => r.ScheduleId == schedYesterday.ScheduleId);
        Assert.Contains(result, r => r.ScheduleId == schedToday.ScheduleId);
        Assert.Contains(result, r => r.ScheduleId == schedTomorrow.ScheduleId);
        Assert.DoesNotContain(result, r => r.ScheduleId == schedTwoDaysAgo.ScheduleId);
        Assert.DoesNotContain(result, r => r.ScheduleId == schedTwoDaysLater.ScheduleId);
    }

    [Fact]
    public async Task Execute_ShouldReturnEmptyList_WhenGetNearbyDatesIsTrueAndNoSchedulesInRange()
    {
        // Arrange;
        (Context context, User user, Company company, _) = await ArrangeSchedulesWithUserAsync(count: 0);

        Client client = ClientMock.Create();
        await Fixture.Save(context, client);

        // Todos fora do range (há 5 dias e daqui 5 dias);
        DateTime today = DateTime.Today;
        Schedule oldSched = ScheduleMock.Create(client.ClientId, company.CompanyId);
        oldSched.DateStart = today.AddDays(-5);
        oldSched.Status = true;
        await Fixture.Save(context, oldSched);

        Schedule futureSched = ScheduleMock.Create(client.ClientId, company.CompanyId);
        futureSched.DateStart = today.AddDays(5);
        futureSched.Status = true;
        await Fixture.Save(context, futureSched);

        GetScheduleByCompanyId sut = CreateSut(context, user);

        // Act;
        List<ScheduleOutput>? result = await sut.Execute(user.UserId, company.CompanyId, null, null, true);

        // Assert;
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #region helpers
    private static async Task<(Context context, User user, Company company, List<Schedule> schedules)> ArrangeSchedulesWithUserAsync(int count = 1)
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

        // Vínculo;
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

        return (context, user, company, schedules);
    }

    private static GetScheduleByCompanyId CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);
        GetClient getClient = new(context, checkIfUserIsLinkedCompanyUser);
        GetCompany getCompany = new(context, checkIfUserIsLinkedCompanyUser);
        Mock<IGenericPublisher> genericPublisherMock = Fixture.CreateGenericPublisher();

        GetScheduleByCompanyId createSchedule = new(new ScheduleBaseDependencies(
            context,
            checkIfUserIsLinkedCompanyUser,
            getClient,
            getCompany,
            genericPublisherMock.Object
        ));

        return createSchedule;
    }
    #endregion
}