using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Orquestra.Application.UseCases.Companies.Base;
using Orquestra.Application.UseCases.Companies.Create;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyInvoices.Create;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.CompanyUsers.GetCurrentMain;
using Orquestra.Application.UseCases.CompanyUsers.Invite;
using Orquestra.Application.UseCases.CompanyUsers.UpdateCurrentMain;
using Orquestra.Application.UseCases.Integrations.Whatsapp.Base;
using Orquestra.Application.UseCases.Integrations.Whatsapp.Create;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Verifications.Create;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Services.Email;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Companies;

public sealed class CreateCompanyTests
{
    [Theory]
    [InlineData(UserRoleEnum.Common)]
    [InlineData(UserRoleEnum.Maintainer)]
    [InlineData(UserRoleEnum.Administrator)]
    public async Task Execute_ShouldCreateCompany_ForAnyUserRole(UserRoleEnum role)
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        user.Role = role;
        await Fixture.Save(context, user);

        Mock<IEmailService> emailServiceMock = Fixture.CreateEmailService();
        CreateCompany sut = CreateSut(context, user, emailServiceMock);

        Company company = CompanyMock.Create();
        CompanyInput input = company.Adapt<CompanyInput>();

        // Act;
        CompanyOutput result = await sut.Execute(user.UserId, input);

        // Assert;
        Assert.NotNull(result);
        Assert.Equal(input.Name, result.Name);
        Assert.Equal(input.Email, result.Email);
        Assert.Equal(input.Phone, result.Phone);
        Assert.Equal(input.CompanyType, result.CompanyType);
        Assert.Equal(input.City, result.City);
        Assert.Equal(input.State, result.State);
        Assert.Equal(input.ZipCode, result.ZipCode);
        Assert.Equal(input.Country, result.Country);
        Assert.Equal(input.Color, result.Color);
        Assert.False(result.Status);
    }

    [Theory]
    [InlineData(UserRoleEnum.Common)]
    [InlineData(UserRoleEnum.Maintainer)]
    [InlineData(UserRoleEnum.Administrator)]
    public async Task Execute_ShouldThrow_WhenCompanyNameIsEmpty(UserRoleEnum role)
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        user.Role = role;
        await Fixture.Save(context, user);

        Mock<IEmailService> emailServiceMock = Fixture.CreateEmailService();
        CreateCompany sut = CreateSut(context, user, emailServiceMock);

        Company company = CompanyMock.Create();
        CompanyInput input = company.Adapt<CompanyInput>();
        input.Name = string.Empty;

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Theory]
    [InlineData(UserRoleEnum.Common)]
    [InlineData(UserRoleEnum.Maintainer)]
    [InlineData(UserRoleEnum.Administrator)]
    public async Task Execute_ShouldThrow_WhenCompanyEmailIsInvalid(UserRoleEnum role)
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        user.Role = role;
        await Fixture.Save(context, user);

        Mock<IEmailService> emailServiceMock = Fixture.CreateEmailService();
        CreateCompany sut = CreateSut(context, user, emailServiceMock);

        Company company = CompanyMock.Create();
        CompanyInput input = company.Adapt<CompanyInput>();
        input.Email = "email-invalido";

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Theory]
    [InlineData(UserRoleEnum.Common)]
    [InlineData(UserRoleEnum.Maintainer)]
    [InlineData(UserRoleEnum.Administrator)]
    public async Task Execute_ShouldSendEmailWithCorrectValues(UserRoleEnum role)
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        user.Role = role;
        await Fixture.Save(context, user);

        Dictionary<string, string>? capturedValues = null;
        Mock<IEmailService> emailServiceMock = Fixture.CreateEmailService(vals => capturedValues = new Dictionary<string, string>(vals));

        CreateCompany sut = CreateSut(context, user, emailServiceMock);

        Company company = CompanyMock.Create();
        CompanyInput input = company.Adapt<CompanyInput>();

        // Act;
        await sut.Execute(user.UserId, input);

        // Assert;
        Assert.NotNull(capturedValues);
        Assert.Equal(input.Name, capturedValues!["[CompanyName]"]);

        emailServiceMock.Verify(x => x.SendEmail(input.Email, It.IsAny<string>(), It.IsAny<string>(), true, It.IsAny<List<string>>()), Times.AtLeastOnce);
    }

    [Theory]
    [InlineData(UserRoleEnum.Common)]
    [InlineData(UserRoleEnum.Maintainer)]
    [InlineData(UserRoleEnum.Administrator)]
    public async Task Execute_ShouldCreateVerificationWithUrlSafeToken(UserRoleEnum role)
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        user.Role = role;
        await Fixture.Save(context, user);

        Mock<IEmailService> emailServiceMock = Fixture.CreateEmailService();
        CreateCompany sut = CreateSut(context, user, emailServiceMock);

        Company company = CompanyMock.Create();
        CompanyInput input = company.Adapt<CompanyInput>();

        // Act;
        CompanyOutput result = await sut.Execute(user.UserId, input);

        // Assert;
        Verification? verification = await context.Verifications.AsNoTracking().FirstOrDefaultAsync(v => v.EntityId == result.CompanyId && v.VerificationType == VerificationTypeEnum.Company);

        Assert.NotNull(verification);
        Assert.Matches("^[A-Za-z0-9_-]+$", verification!.Token); // URL-safe token
        Assert.False(verification.Used);
    }

    [Fact]
    public async Task Execute_ShouldSetCompanySituationAndPlanDatesCorrectly()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Mock<IEmailService> emailService = Fixture.CreateEmailService();
        CreateCompany sut = CreateSut(context, user, emailService);

        Company company = CompanyMock.Create();
        company.PlanType = PlanTypeEnum.Free;

        var input = company.Adapt<CompanyInput>();

        // Act;
        CompanyOutput result = await sut.Execute(user.UserId, input);

        // Assert;
        Company? createdCompany = await context.Companies.FindAsync(result.CompanyId);
        Assert.NotNull(createdCompany);

        Assert.Equal(CompanySituationEnum.Approved, createdCompany!.CompanySituation);
        Assert.NotNull(createdCompany.PlanStartDate);
        Assert.NotNull(createdCompany.PlanEndDate);
    }

    [Theory]
    [InlineData(UserRoleEnum.Common)]
    [InlineData(UserRoleEnum.Maintainer)]
    [InlineData(UserRoleEnum.Administrator)]
    public async Task Execute_ShouldThrow_WhenCompanyNameAlreadyExists(UserRoleEnum role)
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        user.Role = role;
        await Fixture.Save(context, user);

        Mock<IEmailService> emailServiceMock = Fixture.CreateEmailService();
        CreateCompany sut = CreateSut(context, user, emailServiceMock);

        // Empresa já existente;
        Company existingCompany = CompanyMock.Create();
        existingCompany.Name = "Empresa Repetida";
        await Fixture.Save(context, existingCompany);

        // Tentativa de criar outra com mesmo nome;
        Company company = CompanyMock.Create();
        CompanyInput input = company.Adapt<CompanyInput>();
        input.Name = existingCompany.Name; // Duplicado;

        // Act & Assert;
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(user.UserId, input));
        Assert.Contains("já existe", ex.Message.ToLowerInvariant());
    }

    [Theory]
    [InlineData(UserRoleEnum.Common)]
    [InlineData(UserRoleEnum.Maintainer)]
    [InlineData(UserRoleEnum.Administrator)]
    public async Task Execute_ShouldThrow_WhenCompanyEmailAlreadyExists(UserRoleEnum role)
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        user.Role = role;
        await Fixture.Save(context, user);

        Mock<IEmailService> emailServiceMock = Fixture.CreateEmailService();
        CreateCompany sut = CreateSut(context, user, emailServiceMock);

        // Empresa já existente;
        Company existingCompany = CompanyMock.Create();
        existingCompany.Email = "empresa@teste.com";
        await Fixture.Save(context, existingCompany);

        // Tentativa de criar outra com mesmo email;
        Company company = CompanyMock.Create();
        CompanyInput input = company.Adapt<CompanyInput>();
        input.Email = existingCompany.Email; // Duplicado;

        // Act & Assert;
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(user.UserId, input));
        Assert.Contains("já existe", ex.Message.ToLowerInvariant());
    }

    #region helper
    private static CreateCompany CreateSut(Context context, User user, Mock<IEmailService> emailServiceMock)
    {
        IConfiguration config = Fixture.CreateConfiguration();
        IWebHostEnvironment env = Fixture.CreateDevelopmentEnvironment();
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        EnvService envService = new(env, config);
        CreateVerification createVerification = new(context);
        GetUser getUser = new(context);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);
        GetCompany getCompany = new(context, checkIfUserIsLinkedCompanyUser);
        InviteCompanyUser inviteCompanyUser = new(context, envService, createVerification, checkIfUserIsLinkedCompanyUser, getUser, getCompany, emailServiceMock.Object);
        UpdateCurrentMainCompanyUser updateCurrentMainCompanyUser = new(context, checkIfUserIsLinkedCompanyUser);
        CreateCompanyInvoice createCompanyInvoice = new(context, checkIfUserIsLinkedCompanyUser, envService, emailServiceMock.Object);

        GetCurrentMainCompanyUser getCurrentMainCompanyUser = new(context);
        CreateIntegrationWhatsapp createIntegrationWhatsapp = new(new IntegrationWhatsappBaseDependencies(
            context,
            checkIfUserIsLinkedCompanyUser,
            getCurrentMainCompanyUser
        ));

        CreateCompany createCompany = new(new CompanyBaseDependencies(
            context,
            envService,
            createVerification,
            inviteCompanyUser,
            updateCurrentMainCompanyUser,
            getUser,
            emailServiceMock.Object,
            checkIfUserIsLinkedCompanyUser,
            createCompanyInvoice,
            createIntegrationWhatsapp
        ));

        return createCompany;
    }
    #endregion
}