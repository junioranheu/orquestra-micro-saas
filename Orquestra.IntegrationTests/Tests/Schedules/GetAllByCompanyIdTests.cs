using Microsoft.AspNetCore.Http;
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
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Schedules;

public sealed class GetAllByCompanyIdTests
{
    [Fact]
    public async Task Execute_ShouldReturnSchedules_WhenSchedulesExistAndUserLinked()
    {
        // Arrange;
        (Context context, User user, Company company, List<Schedule> schedules) = await ArrangeSchedulesWithUserAsync(count: 3);

        GetScheduleByCompanyId sut = CreateSut(context, user);

        // Act
        List<ScheduleOutput>? result = await sut.Execute(user.UserId, company.CompanyId, null, null);

        // Assert;
        Assert.NotNull(result);
        Assert.Equal(schedules.Count, result.Count);

        foreach (var schedule in schedules)
        {
            Assert.True(schedule.Date > DateTime.MinValue);
            Assert.True(schedule.DurationMinutes > 0);
            Assert.Contains(result, r => r.ScheduleId == schedule.ScheduleId);
        }
    }

    [Fact]
    public async Task Execute_ShouldReturnFilteredSchedules_WhenYearAndMonthProvided()
    {
        // Arrange
        (Context context, User user, Company company, List<Schedule> schedules) = await ArrangeSchedulesWithUserAsync(count: 5);

        // Pega o ano e mês do primeiro schedule
        Schedule targetSchedule = schedules[0];
        int year = targetSchedule.Date.Year;
        int month = targetSchedule.Date.Month;

        GetScheduleByCompanyId sut = CreateSut(context, user);

        // Act;
        List<ScheduleOutput>? result = await sut.Execute(user.UserId, company.CompanyId, year, month);

        // Assert;
        Assert.NotNull(result);
        Assert.All(result, r =>
        {
            Assert.Equal(year, r.Date.Year);
            Assert.Equal(month, r.Date.Month);
        });
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

        // Remove todos os vínculos originais;
        context.CompanyUsers.RemoveRange(context.CompanyUsers);
        await context.SaveChangesAsync();

        // Cria um outro user;
        User user2 = UserMock.Create();
        await Fixture.Save(context, user2);

        // Cria vínculo do usuário com a empresa;
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
        (Context context, User user, Company company, List<Schedule> schedules) = await ArrangeSchedulesWithUserAsync(count: 3);

        // Passa um ano que não existe (ex: 1997);
        int yearNotFound = 1997;

        GetScheduleByCompanyId sut = CreateSut(context, user);

        // Act;
        List<ScheduleOutput>? result = await sut.Execute(user.UserId, company.CompanyId, yearNotFound, null);

        // Assert;
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Execute_ShouldReturnAllSchedules_WhenNoYearOrMonthProvided()
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
    public async Task Execute_ShouldReturnSchedulesFilteredByYear()
    {
        // Arrange;
        (Context context, User user, Company company, List<Schedule> schedules) = await ArrangeSchedulesWithUserAsync(count: 5);

        int targetYear = schedules[0].Date.Year;

        GetScheduleByCompanyId sut = CreateSut(context, user);

        // Act
        List<ScheduleOutput>? result = await sut.Execute(user.UserId, company.CompanyId, targetYear, null);

        // Assert;
        Assert.NotNull(result);
        Assert.All(result, r => Assert.Equal(targetYear, r.Date.Year));
    }

    [Fact]
    public async Task Execute_ShouldReturnSchedulesFilteredByMonth()
    {
        // Arrange;
        (Context context, User user, Company company, List<Schedule> schedules) = await ArrangeSchedulesWithUserAsync(count: 5);

        int targetMonth = schedules[0].Date.Month;

        GetScheduleByCompanyId sut = CreateSut(context, user);

        // Act;
        List<ScheduleOutput>? result = await sut.Execute(user.UserId, company.CompanyId, null, targetMonth);

        // Assert;
        Assert.NotNull(result);
        Assert.All(result, r => Assert.Equal(targetMonth, r.Date.Month));
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

        // Cria vínculo do usuário com a empresa;
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

        GetScheduleByCompanyId createSchedule = new(new ScheduleBaseDependencies(
            context,
            checkIfUserIsLinkedCompanyUser,
            getClient,
            getCompany
        ));

        return createSchedule;
    }
    #endregion
}