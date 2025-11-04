using Mapster;
using Microsoft.AspNetCore.Http;
using Moq;
using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Schedules.Base;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Services.Email;
using Orquestra.Infrastructure.Services.Email.Models;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Schedules;

public sealed class ScheduleBaseTests
{
    private readonly DateTime _DATE = GetDate().Date;

    [Fact]
    public async Task Validate_ShouldThrow_WhenCustomUrlIsInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = new()
        {
            CompanyId = Guid.NewGuid(),
            Name = "Comp",
            Email = "c@c.com",
            PlanType = PlanTypeEnum.Free,
            Status = true
        };

        await context.Companies.AddAsync(company);
        await context.SaveChangesAsync();

        ScheduleBase sut = CreateSut(context, user);

        ScheduleInput input = new()
        {
            CompanyId = company.CompanyId,
            ClientId = Guid.NewGuid(),
            DateStart = _DATE.AddDays(1),
            DateEnd = _DATE.AddDays(1).AddHours(1),
            ScheduleStatus = ScheduleStatusEnum.Scheduled,
            CustomUrl = "this is invalid url"
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenStatusInvalidOnCreate()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = new()
        {
            CompanyId = Guid.NewGuid(),
            Name = "Comp",
            Email = "c@c.com",
            PlanType = PlanTypeEnum.Free,
            Status = true
        };

        await context.Companies.AddAsync(company);
        await context.SaveChangesAsync();

        ScheduleBase sut = CreateSut(context, user);

        ScheduleInput input = new()
        {
            CompanyId = company.CompanyId,
            ClientId = Guid.NewGuid(),
            DateStart = _DATE.AddDays(1),
            DateEnd = _DATE.AddDays(1).AddHours(1),
            ScheduleStatus = ScheduleStatusEnum.Canceled,
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenUpdatingAlreadyCompletedOrCanceled()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Schedule schedule = new()
        {
            ScheduleId = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
            DateStart = _DATE.AddDays(2),
            DateEnd = _DATE.AddDays(2).AddHours(1),
            ScheduleStatus = ScheduleStatusEnum.Completed,
            Status = true,
            CreatedDate = _DATE
        };

        await context.Schedules.AddAsync(schedule);
        await context.SaveChangesAsync();

        ScheduleBase sut = CreateSut(context, user);

        ScheduleInput input = new()
        {
            ScheduleId = schedule.ScheduleId,
            CompanyId = schedule.CompanyId,
            ClientId = schedule.ClientId,
            DateStart = schedule.DateStart,
            DateEnd = schedule.DateEnd,
            ScheduleStatus = ScheduleStatusEnum.Scheduled
        };

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Validate(input, user.UserId, isCreate: false));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenDateIsInThePast_IfMustValidateDate()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = new()
        {
            CompanyId = Guid.NewGuid(),
            Name = "Comp",
            Email = "c@c.com",
            PlanType = PlanTypeEnum.Free,
            Status = true
        };

        await context.Companies.AddAsync(company);
        await context.SaveChangesAsync();

        ScheduleBase sut = CreateSut(context, user);

        ScheduleInput input = new()
        {
            CompanyId = company.CompanyId,
            ClientId = Guid.NewGuid(),
            DateStart = _DATE.AddHours(-1), // Passado;
            DateEnd = _DATE.AddHours(1),
            ScheduleStatus = ScheduleStatusEnum.Scheduled
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, isCreate: true, mustValidateDate: true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenDateEndIsBeforeStart()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = new()
        {
            CompanyId = Guid.NewGuid(),
            Name = "Comp",
            Email = "c@c.com",
            PlanType = PlanTypeEnum.Free,
            Status = true
        };

        await context.Companies.AddAsync(company);
        await context.SaveChangesAsync();

        ScheduleBase sut = CreateSut(context, user);

        DateTime start = _DATE.AddDays(2);
        DateTime end = start.AddHours(-1);

        ScheduleInput input = new()
        {
            CompanyId = company.CompanyId,
            ClientId = Guid.NewGuid(),
            DateStart = start,
            DateEnd = end,
            ScheduleStatus = ScheduleStatusEnum.Scheduled
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenDateEndIsOnDifferentDay()
    {
        // Arrange
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = new()
        {
            CompanyId = Guid.NewGuid(),
            Name = "Comp",
            Email = "c@c.com",
            PlanType = PlanTypeEnum.Free,
            Status = true
        };

        await context.Companies.AddAsync(company);
        await context.SaveChangesAsync();

        ScheduleBase sut = CreateSut(context, user);

        DateTime start = new(2025, 10, 10, 23, 0, 0);
        DateTime end = start.AddHours(2); // Passa para próximo dia;

        ScheduleInput input = new()
        {
            CompanyId = company.CompanyId,
            ClientId = Guid.NewGuid(),
            DateStart = start,
            DateEnd = end,
            ScheduleStatus = ScheduleStatusEnum.Scheduled
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenClientNotFound()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = new()
        {
            CompanyId = Guid.NewGuid(),
            Name = "Comp",
            Email = "c@c.com",
            PlanType = PlanTypeEnum.Free,
            Status = true
        };

        await context.Companies.AddAsync(company);
        await context.SaveChangesAsync();

        ScheduleBase sut = CreateSut(context, user);

        ScheduleInput input = new()
        {
            CompanyId = company.CompanyId,
            ClientId = Guid.NewGuid(), // Não existe;
            DateStart = _DATE.AddDays(1),
            DateEnd = _DATE.AddDays(1).AddHours(1),
            ScheduleStatus = ScheduleStatusEnum.Scheduled
        };

        // Act & Assert (KeyNotFoundException with SystemConsts.Warnings.NotFoundClient);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Validate(input, user.UserId, isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenCompanyNotFoundOrStatusFalse()
    {
        // Arrange: company inexistente;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        ScheduleBase sut = CreateSut(context, user);

        ScheduleInput input = new()
        {
            CompanyId = Guid.NewGuid(), // Não existe;
            ClientId = Guid.NewGuid(),
            DateStart = _DATE.AddDays(1),
            DateEnd = _DATE.AddDays(1).AddHours(1),
            ScheduleStatus = ScheduleStatusEnum.Scheduled
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Validate(input, user.UserId, isCreate: true));

        // Arrange: company existe mas status false;
        var company = new Company
        {
            CompanyId = Guid.NewGuid(),
            Name = "Comp",
            Email = "c@c.com",
            PlanType = PlanTypeEnum.Free,
            Status = false
        };

        await context.Companies.AddAsync(company);
        await context.SaveChangesAsync();

        input.CompanyId = company.CompanyId;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Validate(input, user.UserId, isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenUsersNotLinked()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = new()
        {
            CompanyId = Guid.NewGuid(),
            Name = "Comp",
            Email = "c@c.com",
            PlanType = PlanTypeEnum.Free,
            CompanyType = CompanyTypeEnum.DesenvolvedorSoftware,
            Status = true
        };

        await context.Companies.AddAsync(company);

        // Create a client so client check passes;
        Client client = new()
        {
            ClientId = Guid.NewGuid(),
            FullName = "Cliente",
            CompanyId = company.CompanyId,
            Status = true
        };

        await context.Clients.AddAsync(client);
        await context.SaveChangesAsync();

        ScheduleBase sut = CreateSut(context, user);

        ScheduleInput input = new()
        {
            CompanyId = company.CompanyId,
            ClientId = client.ClientId,
            DateStart = _DATE.AddDays(1),
            DateEnd = _DATE.AddDays(1).AddHours(1),
            ScheduleStatus = ScheduleStatusEnum.Scheduled,
            UsersIds = [Guid.NewGuid()] // User não está vinculado;
        };

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Validate(input, user.UserId, isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenInputIsValid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = new()
        {
            CompanyId = Guid.NewGuid(),
            Name = "Comp",
            Email = "c@c.com",
            PlanType = PlanTypeEnum.Free,
            CompanyType = CompanyTypeEnum.DesenvolvedorSoftware,
            Status = true
        };

        await context.Companies.AddAsync(company);

        CompanyUser companyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator
        };

        await Fixture.Save(context, companyUser);

        Client client = new()
        {
            ClientId = Guid.NewGuid(),
            FullName = "Cliente OK",
            CompanyId = company.CompanyId,
            Status = true
        };

        await context.Clients.AddAsync(client);

        // Create and link a CompanyUser to be valid user for UsersIds;
        User staff = new()
        {
            UserId = Guid.NewGuid(),
            FullName = "Staff",
            Email = "staff@x.com",
            RecoverPasswordQuestion = RecoverPasswordQuestionEnum.BirthCity,
            RecoverPasswordAnswer = GetRandomString(10),
            Status = true
        };

        await context.Users.AddAsync(staff);

        await context.CompanyUsers.AddAsync(new CompanyUser
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = staff.UserId,
            Status = true
        });

        await context.SaveChangesAsync();

        ScheduleBase sut = CreateSut(context, user);

        ScheduleInput input = new()
        {
            CompanyId = company.CompanyId,
            ClientId = client.ClientId,
            DateStart = _DATE.AddDays(2),
            DateEnd = _DATE.AddDays(2).AddHours(1),
            ScheduleStatus = ScheduleStatusEnum.Scheduled,
            UsersIds = [staff.UserId]
        };

        // Act;
        await sut.Validate(input, user.UserId, isCreate: true);

        // Assert: If no exception -> pass. We can assert users normalized;
        Assert.NotNull(input.UsersIds);
        Assert.Single(input.UsersIds);
        Assert.Equal(staff.UserId, input.UsersIds[0]);
    }

    [Fact]
    public async Task CheckForObservations_ShouldReturnExpectedMessages()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Guid companyId = Guid.NewGuid();

        ScheduleOutput schedule = new()
        {
            ScheduleId = Guid.NewGuid(),
            CompanyId = companyId,
            DateStart = default, // Missing date;
            DateEnd = default,
            ScheduleStatus = ScheduleStatusEnum.Scheduled,
            Client = new ClientOutput { FullName = "Cli" }
        };

        ScheduleBase sut = CreateSut(context, user);

        // Act;
        List<string> obs = await sut.CheckForObservations(schedule);

        // Assert;
        Assert.Contains("Agendamento sem data definida.", obs);
        Assert.Contains("Agendamento sem data de encerramento definida.", obs);
    }

    [Fact]
    public async Task CheckForObservations_ShouldDetectConflicts()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Guid companyId = Guid.NewGuid();
        DateTime date = _DATE.AddDays(3).Date.AddHours(10).AddMinutes(30);

        // Existing scheduled (conflict);
        await context.Schedules.AddAsync(new Schedule
        {
            ScheduleId = Guid.NewGuid(),
            CompanyId = companyId,
            DateStart = date,
            DateEnd = date.AddHours(1),
            ScheduleStatus = ScheduleStatusEnum.Scheduled,
            Status = true,
            CreatedDate = _DATE
        });

        await context.SaveChangesAsync();

        ScheduleOutput schedule = new()
        {
            ScheduleId = Guid.NewGuid(),
            CompanyId = companyId,
            DateStart = date,
            DateEnd = date.AddHours(1),
            ScheduleStatus = ScheduleStatusEnum.Scheduled
        };

        ScheduleBase sut = CreateSut(context, user);

        // Act;
        List<string> obs = await sut.CheckForObservations(schedule);

        // Assert;
        Assert.Contains("Existe outro agendamento ativo na mesma data e hora.", obs);
    }

    [Fact]
    public async Task GetUsers_ShouldReturnEmpty_OnNullOrEmpty()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        ScheduleBase sut = CreateSut(context, user);

        // Act;
        UserOutput[] res1 = await sut.GetUsers(null);
        UserOutput[] res2 = await sut.GetUsers(Array.Empty<Guid>());

        // Assert;
        Assert.Empty(res1);
        Assert.Empty(res2);
    }

    [Fact]
    public async Task GetUsers_ShouldReturnUsers_WhenExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        User u1 = new() { UserId = Guid.NewGuid(), FullName = "A", Email = "a@a.com", RecoverPasswordQuestion = RecoverPasswordQuestionEnum.BirthCity, RecoverPasswordAnswer = GetRandomString(10), Status = true };
        User u2 = new() { UserId = Guid.NewGuid(), FullName = "B", Email = "b@b.com", RecoverPasswordQuestion = RecoverPasswordQuestionEnum.BirthCity, RecoverPasswordAnswer = GetRandomString(10), Status = true };
        await context.Users.AddRangeAsync(u1, u2);
        await context.SaveChangesAsync();

        ScheduleBase sut = CreateSut(context, user);

        // Act;
        UserOutput[] result = await sut.GetUsers(new[] { u1.UserId, u2.UserId });

        // Assert;
        Assert.Equal(2, result.Length);
        Assert.Contains(result, x => x.FullName == "A");
        Assert.Contains(result, x => x.FullName == "B");
    }

    [Fact]
    public async Task GetIntegrationWhatsapp_ShouldReturnEmpty_WhenNoScheduleOrNoIntegration()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        User user = UserMock.Create();
        await Fixture.Save(context, user);
        ScheduleBase sut = CreateSut(context, user);

        // Act;
        string empty1 = await sut.GetIntegrationWhatsapp(null);
        Assert.Equal(string.Empty, empty1);

        ScheduleOutput schedule = new()
        {
            ScheduleId = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(),
            DateStart = _DATE.AddDays(1),
            ScheduleStatus = ScheduleStatusEnum.Scheduled,
            Client = new ClientOutput { FullName = "Cli" }
        };

        string empty2 = await sut.GetIntegrationWhatsapp(schedule);
        Assert.Equal(string.Empty, empty2);
    }

    [Fact]
    public async Task GetIntegrationWhatsapp_ShouldReturnTemplatedMessage()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Guid companyId = Guid.NewGuid();

        IntegrationWhatsApp integration = new()
        {
            IntegrationWhatsAppId = Guid.NewGuid(),
            CompanyId = companyId,
            MessageOnScheduleCanceled = "Oi {cliente}, agendamento {data} {hora} - cancelado.",
            MessageOnScheduleConfirmed = "Conf {cliente} {data} {hora}",
            MessageReminderBeforeSchedule = "Lembrete {cliente} {data} {hora}",
            MessageBeforeScheduleAlert = "Hora {cliente} {data} {hora}"
        };

        await context.IntegrationsWhatsApp.AddAsync(integration);

        Company company = new()
        {
            CompanyId = companyId,
            Name = "Empresa",
            Email = "e@e.com",
            Status = true,
            PlanType = PlanTypeEnum.Free
        };

        await context.Companies.AddAsync(company);

        ScheduleOutput schedule = new()
        {
            ScheduleId = Guid.NewGuid(),
            CompanyId = companyId,
            Company = company.Adapt<CompanyOutput>(),
            Client = new ClientOutput { FullName = "Cliente X" },
            DateStart = _DATE.AddDays(1).Date.AddHours(10).AddMinutes(0),
            DateEnd = _DATE.AddDays(1).Date.AddHours(11).AddMinutes(0),
            ScheduleStatus = ScheduleStatusEnum.Scheduled
        };

        await context.SaveChangesAsync();

        ScheduleBase sut = CreateSut(context, user);

        // Act: Confirmed;
        string msgConfirmed = await sut.GetIntegrationWhatsapp(schedule);
        Assert.Contains("Cliente X", msgConfirmed);

        // Act: canceled;
        schedule.ScheduleStatus = ScheduleStatusEnum.Canceled;
        string msgCanceled = await sut.GetIntegrationWhatsapp(schedule);
        Assert.Contains("cancelado", msgCanceled);

        // Act: 1 day before (we emulate diff by making DateStart close);
        schedule.ScheduleStatus = ScheduleStatusEnum.Scheduled;
        schedule.DateStart = _DATE.AddHours(23); // Less than 1 day;
        string? msgReminder = await sut.GetIntegrationWhatsapp(schedule);

        Assert.True(msgReminder is not null);
    }

    #region helpers
    private static ScheduleBase CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);
        GetClient getClient = new(context, checkIfUserIsLinkedCompanyUser);
        GetCompany getCompany = new(context, checkIfUserIsLinkedCompanyUser);

        Mock<IEmailService> emailServiceMock = new();
        emailServiceMock.Setup(x => x.SendEmail(It.IsAny<EmailInput>())).Returns(Task.CompletedTask);

        ScheduleBaseDependencies deps = new(context, checkIfUserIsLinkedCompanyUser, getClient, getCompany, emailServiceMock.Object);
        ScheduleBase scheduleBase = new(deps);

        return scheduleBase;
    }
    #endregion
}