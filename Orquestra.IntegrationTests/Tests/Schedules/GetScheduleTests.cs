using Microsoft.AspNetCore.Http;
using Moq;
using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Schedules.Base;
using Orquestra.Application.UseCases.Schedules.Get;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Services.Email;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Schedules;

public sealed class GetScheduleTests
{
    [Fact]
    public async Task Execute_ShouldReturnSchedule_WhenScheduleExistsAndUserLinked()
    {
        // Arrange;
        (Context context, User user, Schedule schedule) = await ArrangeScheduleWithUserAsync(status: true);

        GetSchedule sut = CreateSut(context, user);

        // Act;
        ScheduleOutput? result = await sut.Execute(user.UserId, schedule.ScheduleId);

        // Assert;
        Assert.NotNull(result);
        Assert.Equal(schedule.ScheduleId, result.ScheduleId);
        Assert.Equal(schedule.CompanyId, result.CompanyId);
        Assert.Equal(schedule.DateStart, result.DateStart);
        Assert.Equal(schedule.DateEnd, result.DateEnd);
        Assert.Equal(schedule.UsersIds?.Length, result.UsersOutput?.Length);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenScheduleDoesNotExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        User user = UserMock.Create();
        await Fixture.Save(context, user);

        GetSchedule sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(user.UserId, Guid.NewGuid()));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserNotLinkedToCompany()
    {
        // Arrange;
        (Context context, User user, Schedule schedule) = await ArrangeScheduleWithUserAsync(status: true);

        // Remove o vínculo do usuário original com a empresa;
        context.CompanyUsers.RemoveRange(context.CompanyUsers);
        await context.SaveChangesAsync();

        // Cria um outro user;
        User user2 = UserMock.Create();
        await Fixture.Save(context, user2);

        // Cria vínculo do usuário com a empresa;
        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = schedule.CompanyId,
            UserId = user2.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        GetSchedule sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, schedule.ScheduleId));
    }

    #region helpers
    private static async Task<(Context context, User user, Schedule schedule)> ArrangeScheduleWithUserAsync(bool status = true)
    {
        Context context = Fixture.CreateContext();

        // Cria user;
        User user = UserMock.Create();
        await Fixture.Save(context, user);

        // Cria company;
        Company company = CompanyMock.Create();
        company.Status = status;
        await Fixture.Save(context, company);

        // Cria client;
        Client client = ClientMock.Create();
        company.Status = status;
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

        // Cria schedule;
        Schedule schedule = ScheduleMock.Create(clientId: client.ClientId, companyId: company.CompanyId);
        schedule.Status = status;
        await Fixture.Save(context, schedule);

        return (context, user, schedule);
    }

    private static GetSchedule CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);
        GetClient getClient = new(context, checkIfUserIsLinkedCompanyUser);
        GetCompany getCompany = new(context, checkIfUserIsLinkedCompanyUser);
        Mock<IEmailService> emailServiceMock = Fixture.CreateEmailService();

        GetSchedule getSchedule = new(new ScheduleBaseDependencies(
            context,
            checkIfUserIsLinkedCompanyUser,
            getClient,
            getCompany,
            emailServiceMock.Object
        ));

        return getSchedule;
    }
    #endregion
}