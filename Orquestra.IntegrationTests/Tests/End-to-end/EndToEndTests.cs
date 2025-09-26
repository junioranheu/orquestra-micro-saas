using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Orquestra.Application.UseCases.Companies.CalculatePrice;
using Orquestra.Application.UseCases.Companies.Create;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.Companies.Verify;
using Orquestra.Application.UseCases.CompanyInvoices.Create;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.CompanyUsers.Invite;
using Orquestra.Application.UseCases.CompanyUsers.UpdateCurrentMain;
using Orquestra.Application.UseCases.CompanyUsers.Verify;
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
using Orquestra.Infrastructure.Services.Email;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.End_to_end;

public sealed class EndToEndTests
{
    [Fact]
    public async Task FullHappyPathFlow()
    {
        // Arrange - Context + common fixtures;
        Context context = Fixture.CreateContext();
        Mock<IEmailService> emailServiceMock = Fixture.CreateEmailService();
        EnvService envService = new(Fixture.CreateDevelopmentEnvironment(), Fixture.CreateConfiguration());
        CreateVerification createVerification = new(context);
        GetUser getUser = new(context);

        // ---------- 1) CREATE USER 
        CreateUser createUser = CreateUserSut(context, envService, createVerification, emailServiceMock.Object, getUser);

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

        // ---------- 1.2) VERIFY USER
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
        CreateCompany createCompany = CreateCompanySut(context, envService, createVerification, emailServiceMock, createdUserAdapt);

        CompanyInput companyInput = new()
        {
            Name = "Orquestra Test Co",
            Email = $"contato.{Guid.NewGuid().ToString()[..6]}@empresa.test",
            Phone = "11999999999",
            Address = "Rua Bartholomeu do Chango",
            City = "São Paulo",
            State = "SP",
            ZipCode = "02726090",
            Country = "Brasil",
            CompanyType = CompanyTypeEnum.Academia,
            Modules = [ModuleEnum.Scheduling]
        };

        CompanyOutput createdCompanyOutput = await createCompany.Execute(createdUser.UserId, companyInput);

        // Verify company persisted;
        Company? dbCompany = await context.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Name == companyInput.Name);
        Assert.NotNull(dbCompany);
        Assert.Equal(companyInput.Email, dbCompany.Email);
        Assert.Contains(ModuleEnum.Scheduling, dbCompany.Modules ?? []);

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

        InviteCompanyUser inviteCompanyUser = InviteCompanyUserSut(context, envService, createVerification, emailServiceMock.Object, createdUserAdapt);

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

        // ---------- 6) CRIA UM INVOICE via CreateCompanyInvoice;
        CreateCompanyInvoice createCompanyInvoice = CreateCompanyInvoiceSut(context, createdUserAdapt);

        ModuleEnum[] newModules = [ModuleEnum.Scheduling];
        CompanyInvoice? invoice = await createCompanyInvoice.Execute(createdUser.UserId, dbCompany.CompanyId, newModules, isCreateCompany: true);

        // Invoice deve existir (não nulo) e persistido;
        Assert.NotNull(invoice);
        CompanyInvoice? dbInvoice = await context.CompanyInvoices.FirstOrDefaultAsync(i => i.CompanyInvoiceId == invoice.CompanyInvoiceId);
        Assert.NotNull(dbInvoice);

        // ---------- 7) VERIFICAÇÕES EXTRAS;
        // 7.1) Invoice amount > 0;
        Assert.True(invoice.Amount > 0, "Invoice amount should be greater than 0");

        // 7.2) Invoice description contém todos os módulos;
        foreach (var module in newModules)
        {
            Assert.Contains(GetEnumDesc(module), invoice.Description);
        }

        // 7.3) Verifica que foi chamado pelo menos X vezes;
        emailServiceMock.Verify(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<IEnumerable<string>?>()), Times.AtLeast(4));

        // 7.4) Verifica chamada específica para invoice;
        emailServiceMock.Verify(x => x.SendEmail(It.Is<string>(to => to == dbCompany.Email), It.Is<string>(subject => subject.Contains("Nova fatura")), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<IEnumerable<string>?>()), Times.Once);
    }

    #region helpers
    private static CreateUser CreateUserSut(Context context, IEnvService envService, CreateVerification createVerification, IEmailService emailService, GetUser getUser)
    {
        CreateUser createUser = new(context, envService, createVerification, emailService, getUser);

        return createUser;
    }

    private static VerifyUser CreateUserVerifySut(Context context)
    {
        GetVerification getVerification = new(context);
        UpdateVerification updateVerification = new(context, getVerification);
        VerifyUser verifyUser = new(context, getVerification, updateVerification);

        return verifyUser;
    }

    private static CreateCompany CreateCompanySut(Context context, IEnvService envService, CreateVerification createVerification, Mock<IEmailService> emailServiceMock, User createdUser)
    {
        // Monta as dependências usadas pelo CreateCompany (repete padrão que você tem nos outros testes)
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(createdUser);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinked = new(getCompanyUserByCompanyId, httpContextAccessor);
        GetUser getUser = new(context);
        var getCompany = new Application.UseCases.Companies.Get.GetCompany(context, checkIfUserIsLinked);
        InviteCompanyUser inviteCompanyUser = new(context, envService, createVerification, checkIfUserIsLinked, getUser, getCompany, emailServiceMock.Object);
        UpdateCurrentMainCompanyUser updateCurrentMain = new(context, checkIfUserIsLinked);
        CalculatePriceModuleCompany calculatePrice = new(context, checkIfUserIsLinked);
        CreateCompanyInvoice createCompanyInvoice = new(context, checkIfUserIsLinked, calculatePrice, envService, emailServiceMock.Object);

        CreateCompany createCompany = new(context, envService, createVerification, inviteCompanyUser, updateCurrentMain, getUser, emailServiceMock.Object, checkIfUserIsLinked, createCompanyInvoice);

        return createCompany;
    }

    private static VerifyCompany CreateCompanyVerifySut(Context context)
    {
        GetVerification getVerification = new(context);
        UpdateVerification updateVerification = new(context, getVerification);
        VerifyCompany verifyCompany = new(context, getVerification, updateVerification);

        return verifyCompany;
    }

    private static InviteCompanyUser InviteCompanyUserSut(Context context, IEnvService envService, CreateVerification createVerification, IEmailService emailService, User currentUser)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(currentUser);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinked = new(getCompanyUserByCompanyId, httpContextAccessor);
        GetUser getUser = new(context);
        var getCompany = new Application.UseCases.Companies.Get.GetCompany(context, checkIfUserIsLinked);

        InviteCompanyUser inviteCompanyUser = new(context, envService, createVerification, checkIfUserIsLinked, getUser, getCompany, emailService);

        return inviteCompanyUser;
    }

    private static VerifyCompanyUser CreateCompanyUserVerifySut(Context context)
    {
        GetVerification getVerification = new(context);
        UpdateVerification updateVerification = new(context, getVerification);
        VerifyCompanyUser verifyCompanyUser = new(context, getVerification, updateVerification);

        return verifyCompanyUser;
    }

    private static CreateCompanyInvoice CreateCompanyInvoiceSut(Context context, User currentUser)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(currentUser);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinked = new(getCompanyUserByCompanyId, httpContextAccessor);
        CalculatePriceModuleCompany calculatePrice = new(context, checkIfUserIsLinked);
        EnvService envService = new(Fixture.CreateDevelopmentEnvironment(), Fixture.CreateConfiguration());
        Mock<IEmailService> emailServiceMock = Fixture.CreateEmailService();

        CreateCompanyInvoice createCompanyInvoice = new(context, checkIfUserIsLinked, calculatePrice, envService, emailServiceMock.Object);

        return createCompanyInvoice;
    }
    #endregion
}