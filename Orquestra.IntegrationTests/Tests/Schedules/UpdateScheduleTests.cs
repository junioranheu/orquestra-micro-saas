using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Schedules.Base;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Application.UseCases.Schedules.Update;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Schedules;

public sealed class UpdateScheduleTests
{
    [Fact]
    public async Task Execute_ShouldUpdateSchedule_WhenInputIsValid()
    {
        // Arrange;
        (Context context, User user, Schedule schedule) = await ArrangeScheduleWithUserAsync();

        ScheduleInput input = new()
        {
            ScheduleId = schedule.ScheduleId,
            CompanyId = schedule.CompanyId,
            ClientId = schedule.ClientId,
            Date = schedule.Date.AddDays(1),
            UsersIds = schedule.UsersIds,
            PaymentType = PaymentTypeEnum.Pix,
            ScheduleStatus = schedule.ScheduleStatus
        };

        UpdateSchedule sut = CreateSut(context, user);

        // Act;
        ScheduleOutput result = await sut.Execute(user.UserId, input);

        // Assert;
        Assert.NotNull(result);
        Assert.Equal(input.ScheduleId, result.ScheduleId);
        Assert.Equal(input.Date, result.Date);
        Assert.Equal(input.PaymentType, result.PaymentType);

        // Confirma que foi salvo no contexto;
        Schedule? saved = await context.Schedules.FindAsync(schedule.ScheduleId);
        Assert.NotNull(saved);
        Assert.Equal(input.Date, saved.Date);
        Assert.Equal(input.PaymentType, saved.PaymentType);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenScheduleDoesNotExist()
    {
        // Arrange;
        (Context context, User user, _) = await ArrangeScheduleWithUserAsync();

        ScheduleInput input = new()
        {
            ScheduleId = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
            Date = GetDate().AddDays(1),
            UsersIds = []
        };

        UpdateSchedule sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserNotLinkedToCompany()
    {
        // Arrange;
        (Context context, User user, Schedule schedule) = await ArrangeScheduleWithUserAsync();

        // Remove vínculo do usuário;
        context.CompanyUsers.RemoveRange(context.CompanyUsers);
        await context.SaveChangesAsync();

        ScheduleInput input = new()
        {
            ScheduleId = schedule.ScheduleId,
            CompanyId = schedule.CompanyId,
            ClientId = schedule.ClientId,
            Date = schedule.Date.AddDays(1),
            UsersIds = schedule.UsersIds,
            PaymentType = schedule.PaymentType,
            ScheduleStatus = schedule.ScheduleStatus
        };

        UpdateSchedule sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, input));
    }

    #region helpers
    private static async Task<(Context context, User user, Schedule schedule)> ArrangeScheduleWithUserAsync()
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

        // Cria schedule;
        Schedule schedule = ScheduleMock.Create(client.ClientId, company.CompanyId);
        schedule.Status = true;
        schedule.UsersIds = [user.UserId];
        await Fixture.Save(context, schedule);

        return (context, user, schedule);
    }

    private static UpdateSchedule CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);
        GetClient getClient = new(context, checkIfUserIsLinkedCompanyUser);
        GetCompany getCompany = new(context, checkIfUserIsLinkedCompanyUser);

        UpdateSchedule sut = new(new ScheduleBaseDependencies(
            context,
            checkIfUserIsLinkedCompanyUser,
            getClient,
            getCompany
        ));

        return sut;
    }
    #endregion
}