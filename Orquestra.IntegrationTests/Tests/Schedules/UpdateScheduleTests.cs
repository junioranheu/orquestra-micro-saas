using Mapster;
using Microsoft.AspNetCore.Http;
using Moq;
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
using Orquestra.Infrastructure.Services.Email;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Schedules;

public sealed class UpdateScheduleTests
{
    [Fact]
    public async Task Execute_ShouldUpdateSchedule_WhenInputIsValid()
    {
        (Context context, User user, Schedule schedule) = await ArrangeScheduleWithUserAsync();

        ScheduleInput input = schedule.Adapt<ScheduleInput>();

        UpdateSchedule sut = CreateSut(context, user);

        ScheduleOutput result = await sut.Execute(user.UserId, input);

        Assert.NotNull(result);
        Assert.Equal(input.ScheduleId, result.ScheduleId);
        Assert.Equal(input.DateStart, result.DateStart);
        Assert.Equal(input.PaymentType, result.PaymentType);

        Schedule? saved = await context.Schedules.FindAsync(schedule.ScheduleId);
        Assert.NotNull(saved);
        Assert.Equal(input.DateStart, saved.DateStart);
        Assert.Equal(input.PaymentType, saved.PaymentType);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenDateEndIsBeforeDateStart()
    {
        (Context context, User user, Schedule schedule) = await ArrangeScheduleWithUserAsync();

        ScheduleInput input = new()
        {
            ScheduleId = schedule.ScheduleId,
            CompanyId = schedule.CompanyId,
            ClientId = schedule.ClientId,
            DateStart = schedule.DateStart.AddDays(1).AddHours(10),
            DateEnd = schedule.DateEnd.AddYears(-1),
            UsersIds = schedule.UsersIds,
            PaymentType = schedule.PaymentType,
            ScheduleStatus = schedule.ScheduleStatus
        };

        UpdateSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenDateStartIsPastCurrentTimeInDay()
    {
        (Context context, User user, Schedule schedule) = await ArrangeScheduleWithUserAsync();

        ScheduleInput input = schedule.Adapt<ScheduleInput>();
        input.DateEnd = input.DateStart.AddMinutes(-30);

        UpdateSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenDurationExceedsEndOfDay()
    {
        (Context context, User user, Schedule schedule) = await ArrangeScheduleWithUserAsync();

        DateTime scheduleDate = GetDate().AddDays(1).Date.AddHours(23).AddMinutes(30);

        ScheduleInput input = new()
        {
            ScheduleId = schedule.ScheduleId,
            CompanyId = schedule.CompanyId,
            ClientId = schedule.ClientId,
            DateStart = scheduleDate,
            DateEnd = scheduleDate.AddDays(1), // Ultrapassa meia-noite;
            UsersIds = schedule.UsersIds,
            PaymentType = schedule.PaymentType,
            ScheduleStatus = schedule.ScheduleStatus
        };

        UpdateSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenScheduleDoesNotExist()
    {
        (Context context, User user, Schedule _) = await ArrangeScheduleWithUserAsync();

        ScheduleInput input = new()
        {
            ScheduleId = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
            DateStart = GetDate().AddDays(1),
            DateEnd = GetDate().AddDays(1).AddHours(1),
            UsersIds = []
        };

        UpdateSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserNotLinkedToCompany()
    {
        (Context context, User user, Schedule schedule) = await ArrangeScheduleWithUserAsync();

        context.CompanyUsers.RemoveRange(context.CompanyUsers);
        await context.SaveChangesAsync();

        // Cria um outro usuário vinculado à empresa, mas não o user original;
        User user2 = UserMock.Create();
        await Fixture.Save(context, user2);

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = schedule.CompanyId,
            UserId = user2.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        ScheduleInput input = new()
        {
            ScheduleId = schedule.ScheduleId,
            CompanyId = schedule.CompanyId,
            ClientId = schedule.ClientId,
            DateStart = schedule.DateStart.AddDays(1),
            DateEnd = schedule.DateEnd.AddDays(1),
            UsersIds = schedule.UsersIds,
            PaymentType = schedule.PaymentType,
            ScheduleStatus = schedule.ScheduleStatus
        };

        UpdateSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenCustomUrlIsInvalid()
    {
        (Context context, User user, Schedule schedule) = await ArrangeScheduleWithUserAsync();

        var input = schedule.Adapt<ScheduleInput>();
        input.CustomUrl = "ftp://invalid-url";

        UpdateSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserNotLinkedToCompanyOnUpdate()
    {
        (Context context, User user, Schedule schedule) = await ArrangeScheduleWithUserAsync();

        context.CompanyUsers.RemoveRange(context.CompanyUsers);
        await context.SaveChangesAsync();

        var input = schedule.Adapt<ScheduleInput>();

        UpdateSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenNoValidUsersOnUpdate()
    {
        (Context context, User user, Schedule schedule) = await ArrangeScheduleWithUserAsync();

        User invalidUser = UserMock.Create();
        await Fixture.Save(context, invalidUser);

        var input = schedule.Adapt<ScheduleInput>();
        input.UsersIds = [invalidUser.UserId];

        UpdateSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenDateStartIsPastOnUpdate()
    {
        (Context context, User user, Schedule schedule) = await ArrangeScheduleWithUserAsync();

        var input = schedule.Adapt<ScheduleInput>();
        input.DateStart = GetDate().AddMinutes(-10);
        input.DateEnd = GetDate().AddMinutes(10);

        UpdateSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenDateEndBeforeDateStartOnUpdate()
    {
        (Context context, User user, Schedule schedule) = await ArrangeScheduleWithUserAsync();

        var input = schedule.Adapt<ScheduleInput>();
        input.DateEnd = input.DateStart.AddHours(-1);

        UpdateSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenDateEndCrossesDayBoundaryOnUpdate()
    {
        (Context context, User user, Schedule schedule) = await ArrangeScheduleWithUserAsync();

        DateTime scheduleDate = GetDate().AddDays(1).Date.AddHours(23).AddMinutes(30);

        var input = schedule.Adapt<ScheduleInput>();
        input.DateStart = scheduleDate;
        input.DateEnd = scheduleDate.AddHours(12);

        UpdateSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenCompanyOrClientNotFound()
    {
        (Context context, User user, Schedule schedule) = await ArrangeScheduleWithUserAsync();

        var input = schedule.Adapt<ScheduleInput>();
        input.CompanyId = Guid.NewGuid(); // Empresa inválida;

        UpdateSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task CheckForObservations_ShouldReturnObservationsOnUpdate()
    {
        (Context context, User user, Schedule schedule) = await ArrangeScheduleWithUserAsync();

        UpdateSchedule sut = CreateSut(context, user);

        var scheduleOutput = schedule.Adapt<ScheduleOutput>();
        scheduleOutput.DateStart = GetDate().AddDays(-1); // Data passada;
        scheduleOutput.DateEnd = default; // Sem data final;

        List<string> observations = await sut.CheckForObservations(scheduleOutput);
        Assert.NotNull(observations);
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
        Mock<IEmailService> emailServiceMock = Fixture.CreateEmailService();

        UpdateSchedule sut = new(new ScheduleBaseDependencies(
            context,
            checkIfUserIsLinkedCompanyUser,
            getClient,
            getCompany,
            emailServiceMock.Object
        ));

        return sut;
    }
    #endregion
}