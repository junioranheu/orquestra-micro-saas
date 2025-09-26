using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Schedules.Base;
using Orquestra.Application.UseCases.Schedules.Create;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Schedules;

public sealed class CreateScheduleTests
{
    [Fact]
    public async Task Execute_ShouldCreateSchedule_WhenInputIsValid()
    {
        // Arrange;
        (Context context, User user, Company company, Client client) = await ArrangeScheduleDependenciesAsync();

        ScheduleInput input = new()
        {
            CompanyId = company.CompanyId,
            ClientId = client.ClientId,
            DateStart = GetDate().AddDays(1).AddHours(10),
            DateEnd = GetDate().AddDays(1).AddHours(10).AddHours(1),
            UsersIds = [user.UserId],
            ScheduleStatus = ScheduleStatusEnum.Scheduled
        };

        CreateSchedule sut = CreateSut(context, user);

        // Act;
        ScheduleOutput result = await sut.Execute(user.UserId, input);

        // Assert;
        Assert.NotNull(result);
        Assert.Equal(input.CompanyId, result.CompanyId);
        Assert.Equal(input.ClientId, result.ClientId);
        Assert.Equal(input.UsersIds.Length, result.UsersOutput?.Length);

        // Confirma que foi salvo no contexto;
        Schedule? saved = await context.Schedules.FindAsync(result.ScheduleId);
        Assert.NotNull(saved);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenDateEndIsInvalid()
    {
        // Arrange;
        (Context context, User user, Company company, Client client) = await ArrangeScheduleDependenciesAsync();

        ScheduleInput input = new()
        {
            CompanyId = company.CompanyId,
            ClientId = client.ClientId,
            DateStart = GetDate().AddDays(1).AddHours(10),
            DateEnd = GetDate().AddDays(1).AddHours(10).AddHours(-22),
            UsersIds = [user.UserId],
            ScheduleStatus = ScheduleStatusEnum.Scheduled
        };

        CreateSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenCustomUrlIsNotHttps()
    {
        (Context context, User user, Company company, Client client) = await ArrangeScheduleDependenciesAsync();

        ScheduleInput input = new()
        {
            CompanyId = company.CompanyId,
            ClientId = client.ClientId,
            DateStart = GetDate().AddDays(1).AddHours(10),
            DateEnd = GetDate().AddDays(1).AddHours(10).AddHours(1),
            UsersIds = [user.UserId],
            ScheduleStatus = ScheduleStatusEnum.Scheduled,
            CustomUrl = "aea"
        };

        CreateSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenDataEndIsBeforeDateStart()
    {
        (Context context, User user, Company company, Client client) = await ArrangeScheduleDependenciesAsync();

        ScheduleInput input = new()
        {
            CompanyId = company.CompanyId,
            ClientId = client.ClientId,
            DateStart = GetDate().AddDays(1).AddHours(10),
            DateEnd = GetDate().AddDays(-10), // Inválido;
            UsersIds = [user.UserId],
            ScheduleStatus = ScheduleStatusEnum.Scheduled
        };

        CreateSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenDateStartIsPastCurrentTimeInDay()
    {
        (Context context, User user, Company company, Client client) = await ArrangeScheduleDependenciesAsync();

        DateTime scheduleDate = GetDate().AddMinutes(-30);

        ScheduleInput input = new()
        {
            CompanyId = company.CompanyId,
            ClientId = client.ClientId,
            DateStart = scheduleDate,
            DateEnd = scheduleDate.AddMinutes(1),
            UsersIds = [user.UserId],
            ScheduleStatus = ScheduleStatusEnum.Scheduled
        };

        CreateSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenDurationExceedsEndOfDay()
    {
        (Context context, User user, Company company, Client client) = await ArrangeScheduleDependenciesAsync();

        // Coloca hora final próxima da meia-noite;
        DateTime scheduleDate = GetDate().AddDays(1).Date.AddHours(23).AddMinutes(30);

        ScheduleInput input = new()
        {
            CompanyId = company.CompanyId,
            ClientId = client.ClientId,
            DateStart = scheduleDate,
            DateEnd = scheduleDate.AddDays(1),
            UsersIds = [user.UserId],
            ScheduleStatus = ScheduleStatusEnum.Scheduled
        };

        CreateSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserNotLinkedToCompany()
    {
        // Arrange;
        (Context context, User user, Company company, Client client) = await ArrangeScheduleDependenciesAsync();

        // Remove todos os vínculos do usuário original;
        context.CompanyUsers.RemoveRange(context.CompanyUsers);
        await context.SaveChangesAsync();

        // Cria um outro usuário vinculado à empresa, mas não o user original;
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

        ScheduleInput input = new()
        {
            CompanyId = company.CompanyId,
            ClientId = client.ClientId,
            DateStart = GetDate().AddDays(1),
            DateEnd = GetDate().AddDays(1).AddMinutes(1),
            UsersIds = [user.UserId],
            ScheduleStatus = ScheduleStatusEnum.Scheduled
        };

        CreateSchedule sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenCustomUrlIsInvalid()
    {
        (Context context, User user, Company company, Client client) = await ArrangeScheduleDependenciesAsync();

        ScheduleInput input = new()
        {
            CompanyId = company.CompanyId,
            ClientId = client.ClientId,
            DateStart = GetDate().AddDays(1).AddHours(10),
            DateEnd = GetDate().AddDays(1).AddHours(11),
            UsersIds = [user.UserId],
            ScheduleStatus = ScheduleStatusEnum.Scheduled,
            CustomUrl = "ftp://invalid-url"
        };

        CreateSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenScheduleStatusIsNotScheduled_OnCreate()
    {
        (Context context, User user, Company company, Client client) = await ArrangeScheduleDependenciesAsync();

        ScheduleInput input = new()
        {
            CompanyId = company.CompanyId,
            ClientId = client.ClientId,
            DateStart = GetDate().AddDays(1),
            DateEnd = GetDate().AddDays(1).AddHours(1),
            UsersIds = [user.UserId],
            ScheduleStatus = ScheduleStatusEnum.Completed
        };

        CreateSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenUserNotLinkedToCompany()
    {
        (Context context, User user, Company company, Client client) = await ArrangeScheduleDependenciesAsync();

        context.CompanyUsers.RemoveRange(context.CompanyUsers);
        await context.SaveChangesAsync();

        ScheduleInput input = new()
        {
            CompanyId = company.CompanyId,
            ClientId = client.ClientId,
            DateStart = GetDate().AddDays(1),
            DateEnd = GetDate().AddDays(1).AddHours(1),
            UsersIds = [user.UserId],
            ScheduleStatus = ScheduleStatusEnum.Scheduled
        };

        CreateSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Validate(input, user.UserId, true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenNoValidUsers()
    {
        (Context context, User user, Company company, Client client) = await ArrangeScheduleDependenciesAsync();

        User invalidUser = UserMock.Create();
        await Fixture.Save(context, invalidUser);

        ScheduleInput input = new()
        {
            CompanyId = company.CompanyId,
            ClientId = client.ClientId,
            DateStart = GetDate().AddDays(1),
            DateEnd = GetDate().AddDays(1).AddHours(1),
            UsersIds = [invalidUser.UserId],
            ScheduleStatus = ScheduleStatusEnum.Scheduled
        };

        CreateSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Validate(input, user.UserId, true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenDateStartIsPast()
    {
        (Context context, User user, Company company, Client client) = await ArrangeScheduleDependenciesAsync();

        ScheduleInput input = new()
        {
            CompanyId = company.CompanyId,
            ClientId = client.ClientId,
            DateStart = GetDate().AddMinutes(-10),
            DateEnd = GetDate().AddMinutes(10),
            UsersIds = [user.UserId],
            ScheduleStatus = ScheduleStatusEnum.Scheduled
        };

        CreateSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenDateEndBeforeDateStart()
    {
        (Context context, User user, Company company, Client client) = await ArrangeScheduleDependenciesAsync();

        ScheduleInput input = new()
        {
            CompanyId = company.CompanyId,
            ClientId = client.ClientId,
            DateStart = GetDate().AddDays(1).AddHours(10),
            DateEnd = GetDate().AddDays(1).AddHours(9),
            UsersIds = [user.UserId],
            ScheduleStatus = ScheduleStatusEnum.Scheduled
        };

        CreateSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenDateEndCrossesDayBoundary()
    {
        (Context context, User user, Company company, Client client) = await ArrangeScheduleDependenciesAsync();

        DateTime scheduleDate = GetDate().AddDays(1).Date.AddHours(23).AddMinutes(30);

        ScheduleInput input = new()
        {
            CompanyId = company.CompanyId,
            ClientId = client.ClientId,
            DateStart = scheduleDate,
            DateEnd = scheduleDate.AddHours(12),
            UsersIds = [user.UserId],
            ScheduleStatus = ScheduleStatusEnum.Scheduled
        };

        CreateSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, true));
    }

    [Fact]
    public async Task CheckForObservations_ShouldReturnObservations()
    {
        (Context context, User user, Company company, Client _) = await ArrangeScheduleDependenciesAsync();

        CreateSchedule sut = CreateSut(context, user);

        ScheduleOutput schedule = new()
        {
            CompanyId = company.CompanyId,
            DateStart = GetDate().AddDays(-1),
            DateEnd = DateTime.MinValue,
            ScheduleStatus = ScheduleStatusEnum.Scheduled
        };

        List<string> observations = await sut.CheckForObservations(schedule);
        Assert.NotNull(observations);
    }

    #region helpers
    private static async Task<(Context context, User user, Company company, Client client)> ArrangeScheduleDependenciesAsync()
    {
        Context context = Fixture.CreateContext();

        // Cria usuário;
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

        return (context, user, company, client);
    }

    private static CreateSchedule CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);
        GetClient getClient = new(context, checkIfUserIsLinkedCompanyUser);
        GetCompany getCompany = new(context, checkIfUserIsLinkedCompanyUser);

        CreateSchedule getSchedule = new(new ScheduleBaseDependencies(
            context,
            checkIfUserIsLinkedCompanyUser,
            getClient,
            getCompany
        ));

        return getSchedule;
    }
    #endregion
}