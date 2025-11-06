using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Orquestra.Application.UseCases.Companies.Base;
using Orquestra.Application.UseCases.Companies.Create;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.Companies.Verify;
using Orquestra.Application.UseCases.CompanyInvoices.Create;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.CompanyUsers.Invite;
using Orquestra.Application.UseCases.CompanyUsers.UpdateCurrentMain;
using Orquestra.Application.UseCases.CompanyUsers.Verify;
using Orquestra.Application.UseCases.Integrations.WhatsApp.Base;
using Orquestra.Application.UseCases.Integrations.WhatsApp.Create;
using Orquestra.Application.UseCases.Users.Create;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Application.UseCases.Users.Verify;
using Orquestra.Application.UseCases.Verifications.Create;
using Orquestra.Application.UseCases.Verifications.Get;
using Orquestra.Application.UseCases.Verifications.Update;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Messaging.Publishers;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.Infrastructure.Services.Sms;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.End_to_end;

public sealed class EndToEndTests
{
    [Fact]
    public async Task FullHappyPathFlow()
    {
        // Arrange - Context + common fixtures;
        Context context = Fixture.CreateContext();
        Mock<IGenericPublisher> genericPublisherMock = Fixture.CreateGenericPublisher();
        EnvService envService = new(Fixture.CreateDevelopmentEnvironment(), Fixture.CreateConfiguration());
        CreateVerification createVerification = new(context);
        GetUser getUser = new(context);

        // ---------- 1) CREATE USER 
        CreateUser createUser = CreateUserSut(context, envService, createVerification, genericPublisherMock.Object, getUser);

        UserInput newUserInput = new()
        {
            FullName = "Junior Souza",
            Email = $"junior.{Guid.NewGuid().ToString()[..8]}@test.local",
            Password = "Senha123@"
        };

        // Execute CreateUser use case;
        UserOutput createdUser = await createUser.Execute(newUserInput);
        User createdUserAdapt = createdUser.Adapt<User>();

        // Assert user exists in DB;
        User? dbUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == newUserInput.Email);

        Assert.NotNull(dbUser);
        Assert.Equal(newUserInput.FullName, dbUser.FullName);

        // ---------- 1.2) VERIFY USER;
        Verification? userVerification = await context.Verifications.AsNoTracking().
        FirstOrDefaultAsync(x =>
            x.EntityType == nameof(User) &&
            x.VerificationType == VerificationTypeEnum.User &&
            x.Used == false
        );

        Assert.NotNull(userVerification);

        VerifyUser verifyUser = CreateUserVerifySut(context);

        try
        {
            await verifyUser.Execute(userVerification.Token);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Expected no exception, but got: {ex.Message}");
        }

        // ---------- 2) CREATE COMPANY (associa o usuário como primeiro administrador);
        CreateCompany createCompany = CreateCompanySut(context, envService, createVerification, genericPublisherMock, createdUserAdapt);

        CompanyInput companyInput = new()
        {
            Name = "Orquestra Test Co",
            Email = $"contato.{Guid.NewGuid().ToString()[..6]}@empresa.test",
            Phone = "12982716339",
            Address = "Rua Bartholomeu do Chango",
            City = "São Paulo",
            State = "SP",
            ZipCode = "02726090",
            Country = "Brasil",
            CompanyType = CompanyTypeEnum.Freelancer,
            PlanType = PlanTypeEnum.Free
        };

        CompanyOutput createdCompanyOutput = await createCompany.Execute(createdUser.UserId, companyInput);

        // Verify company persisted;
        Company? dbCompany = await context.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Name == companyInput.Name);
        Assert.NotNull(dbCompany);
        Assert.Equal(companyInput.Email, dbCompany.Email);
        Assert.Equal(PlanTypeEnum.Free, dbCompany.PlanType);

        // Verify CompanyUser (first admin) exists and is linked;
        CompanyUser? dbCompanyUser = await context.CompanyUsers.FirstOrDefaultAsync(cu => cu.CompanyId == dbCompany.CompanyId && cu.UserId == createdUser.UserId);
        Assert.NotNull(dbCompanyUser);
        Assert.Equal(CompanyUserRoleEnum.Administrator, dbCompanyUser!.CompanyUserRole);

        // ---------- 2.2) VERIFY COMPANY;
        Verification? companyVerification = await context.Verifications.AsNoTracking().
        FirstOrDefaultAsync(x =>
            x.EntityType == nameof(Company) &&
            x.VerificationType == VerificationTypeEnum.Company &&
            x.Used == false
        );

        Assert.NotNull(companyVerification);

        VerifyCompany verifyCompany = CreateCompanyVerifySut(context);

        try
        {
            await verifyCompany.Execute(companyVerification.Token);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Expected no exception, but got: {ex.Message}");
        }

        // ---------- 3) INVITE / ADD ANOTHER USER TO COMPANY (CompanyUser);
        User invitedUser = UserMock.Create();
        await Fixture.Save(context, invitedUser);

        InviteCompanyUser inviteCompanyUser = InviteCompanyUserSut(context, envService, createVerification, genericPublisherMock.Object, createdUserAdapt);

        // Execute invite (this will create CompanyUser and possibly verification+email);
        await inviteCompanyUser.Execute(createdUser.UserId, dbCompany.CompanyId, invitedUser.Email, isFirstAdministrator: false);

        // Confirm invited user linked to company;
        CompanyUser? invitedCompanyUser = await context.CompanyUsers.FirstOrDefaultAsync(cu => cu.CompanyId == dbCompany.CompanyId && cu.UserId == invitedUser.UserId);
        Assert.NotNull(invitedCompanyUser);

        // ---------- 3.2) VERIFY COMPANY;
        Verification? companyUserVerification = await context.Verifications.AsNoTracking().
        FirstOrDefaultAsync(x =>
            x.EntityType == nameof(CompanyUser) &&
            x.VerificationType == VerificationTypeEnum.CompanyUser &&
            x.Used == false
        );

        Assert.NotNull(companyUserVerification);

        VerifyCompanyUser verifyCompanyUser = CreateCompanyUserVerifySut(context);

        try
        {
            await verifyCompanyUser.Execute(companyUserVerification.Token);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Expected no exception, but got: {ex.Message}");
        }

        // ---------- 4) CREATE CUSTOMER (cliente da empresa);
        Client client = ClientMock.Create();
        await Fixture.Save(context, client);

        // Verifica cliente persistido;
        Client? dbCustomer = await context.Clients.FirstOrDefaultAsync(c => c.ClientId == client.ClientId);
        Assert.NotNull(dbCustomer);
        Assert.Equal(client.FullName, dbCustomer.FullName);

        // ---------- 5) CREATE SCHEDULE para o cliente;
        Schedule schedule = ScheduleMock.Create(clientId: client.ClientId, companyId: dbCompany.CompanyId);
        await Fixture.Save(context, schedule);

        // Verifica schedule persistido;
        Schedule? dbSchedule = await context.Schedules.FirstOrDefaultAsync(s => s.ScheduleId == schedule.ScheduleId);
        Assert.NotNull(dbSchedule);
        Assert.Equal(schedule.DateStart, dbSchedule.DateStart);
        Assert.Equal(schedule.DateEnd, dbSchedule.DateEnd);
        Assert.Equal(dbCustomer.ClientId, dbSchedule.ClientId);
    }

    #region helpers
    private static CreateUser CreateUserSut(Context context, IEnvService envService, CreateVerification createVerification, IGenericPublisher genericPublisher, GetUser getUser)
    {
        CreateUser createUser = new(context, envService, createVerification, getUser, genericPublisher);

        return createUser;
    }

    private static VerifyUser CreateUserVerifySut(Context context)
    {
        GetVerification getVerification = new(context);
        UpdateVerification updateVerification = new(context, getVerification);
        VerifyUser verifyUser = new(context, getVerification, updateVerification);

        return verifyUser;
    }

    private static CreateCompany CreateCompanySut(Context context, IEnvService envService, CreateVerification createVerification, Mock<IGenericPublisher> genericPublisherMock, User createdUser)
    {
        // Monta as dependências usadas pelo CreateCompany (repete padrão que você tem nos outros testes)
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(createdUser);
        GetAllCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);
        GetUser getUser = new(context);
        GetCompany getCompany = new(context, checkIfUserIsLinkedCompanyUser);
        InviteCompanyUser inviteCompanyUser = new(context, envService, createVerification, checkIfUserIsLinkedCompanyUser, getUser, getCompany, genericPublisherMock.Object);
        UpdateCurrentMainCompanyUser updateCurrentMainCompanyUser = new(context, checkIfUserIsLinkedCompanyUser);
        CreateCompanyInvoice createCompanyInvoice = new(context, checkIfUserIsLinkedCompanyUser, envService, genericPublisherMock.Object);

        Mock<ISmsService> smsServiceMock = new();
        smsServiceMock.Setup(x => x.SendSms(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>())).ReturnsAsync("OK");

        CreateIntegrationWhatsApp createIntegrationWhatsApp = new(new IntegrationWhatsAppBaseDependencies(
            context,
            checkIfUserIsLinkedCompanyUser,
            smsServiceMock.Object
        ));

        CreateCompany createCompany = new(new CompanyBaseDependencies(
           context,
           envService,
           createVerification,
           inviteCompanyUser,
           updateCurrentMainCompanyUser,
           getUser,
           checkIfUserIsLinkedCompanyUser,
           createCompanyInvoice,
           createIntegrationWhatsApp,
           genericPublisherMock.Object
       ));

        return createCompany;
    }

    private static VerifyCompany CreateCompanyVerifySut(Context context)
    {
        GetVerification getVerification = new(context);
        UpdateVerification updateVerification = new(context, getVerification);
        VerifyCompany verifyCompany = new(context, getVerification, updateVerification);

        return verifyCompany;
    }

    private static InviteCompanyUser InviteCompanyUserSut(Context context, IEnvService envService, CreateVerification createVerification, IGenericPublisher publish, User currentUser)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(currentUser);
        GetAllCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinked = new(getCompanyUserByCompanyId, httpContextAccessor);
        GetUser getUser = new(context);
        GetCompany getCompany = new(context, checkIfUserIsLinked);

        InviteCompanyUser inviteCompanyUser = new(context, envService, createVerification, checkIfUserIsLinked, getUser, getCompany, publish);

        return inviteCompanyUser;
    }

    private static VerifyCompanyUser CreateCompanyUserVerifySut(Context context)
    {
        GetVerification getVerification = new(context);
        UpdateVerification updateVerification = new(context, getVerification);
        VerifyCompanyUser verifyCompanyUser = new(context, getVerification, updateVerification);

        return verifyCompanyUser;
    }
    #endregion
}