using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Orquestra.Application.UseCases.Companies.Base;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.Companies.Update;
using Orquestra.Application.UseCases.CompanyInvoices.Create;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.CompanyUsers.GetCurrentMain;
using Orquestra.Application.UseCases.CompanyUsers.Invite;
using Orquestra.Application.UseCases.CompanyUsers.UpdateCurrentMain;
using Orquestra.Application.UseCases.Integrations.WhatsApp.Base;
using Orquestra.Application.UseCases.Integrations.WhatsApp.Create;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Verifications.Create;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Services.Email;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.Infrastructure.Services.Sms;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Companies;

public sealed class UpdateCompanyTests
{
    [Theory]
    [InlineData(UserRoleEnum.Common)]
    [InlineData(UserRoleEnum.Maintainer)]
    [InlineData(UserRoleEnum.Administrator)]
    public async Task Execute_ShouldUpdateCompany_ForAnyUserRole(UserRoleEnum role)
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        user.Role = role;
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        Mock<IEmailService> emailService = Fixture.CreateEmailService();
        UpdateCompany sut = CreateSut(context, user, emailService);

        CompanyInput input = company.Adapt<CompanyInput>();

        input.Name = "Junior Souza";
        input.Email = "juninhoplay@gmail.com";
        input.Phone = "12 982716322";
        input.City = "Lorena";

        // Act;
        CompanyOutput result = await sut.Execute(user.UserId, input);

        // Assert;
        Assert.NotNull(result);
        Assert.Equal(input.Name, result.Name);
        Assert.Equal(input.Email, result.Email);
        Assert.Equal(input.Phone, result.Phone);
        Assert.Equal(input.City, result.City);

        Company? updatedCompany = await context.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.CompanyId == company.CompanyId);
        Assert.NotNull(updatedCompany);
        Assert.Equal(input.Name, updatedCompany!.Name);
        Assert.Equal(input.Email, updatedCompany.Email);
        Assert.Equal(input.Phone, updatedCompany.Phone);
        Assert.Equal(input.City, updatedCompany.City);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenCompanyNotFound()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Mock<IEmailService> emailService = Fixture.CreateEmailService();
        UpdateCompany sut = CreateSut(context, user, emailService);

        CompanyInput input = new()
        {
            CompanyId = Guid.NewGuid(), // ID inexistente;
            Name = "Nome",
            Email = "email@teste.com"
        };

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(user.UserId, input));
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

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        Mock<IEmailService> emailService = Fixture.CreateEmailService();
        UpdateCompany sut = CreateSut(context, user, emailService);

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

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        Mock<IEmailService> emailService = Fixture.CreateEmailService();
        UpdateCompany sut = CreateSut(context, user, emailService);

        CompanyInput input = company.Adapt<CompanyInput>();
        input.Email = "invalid-email";

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserHasNoPermission()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User user = UserMock.Create();
        user.Role = UserRoleEnum.Common;
        await Fixture.Save(context, user);

        CompanyUser companyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        User otherUser = UserMock.Create();
        otherUser.Role = UserRoleEnum.Common;
        await Fixture.Save(context, otherUser);

        Mock<IEmailService> emailService = Fixture.CreateEmailService();
        UpdateCompany sut = CreateSut(context, user, emailService);

        CompanyInput input = company.Adapt<CompanyInput>();

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(otherUser.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldUpdateOnlyProvidedFields()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        Mock<IEmailService> emailService = Fixture.CreateEmailService();
        UpdateCompany sut = CreateSut(context, user, emailService);

        CompanyInput input = company.Adapt<CompanyInput>();

        const string newName = "Juninho Play"; // Apenas nome alterado;
        input.Name = newName;

        // Act;
        CompanyOutput result = await sut.Execute(user.UserId, input);

        // Assert;
        Company? updatedCompany = await context.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.CompanyId == company.CompanyId);
        Assert.NotNull(updatedCompany);
        Assert.Equal(newName, updatedCompany.Name);
        Assert.Equal(company.Email, updatedCompany.Email); // Email permanece igual (antigo);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenMemberIsNotAdministrator()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        user.Role = UserRoleEnum.Common;
        await Fixture.Save(context, user);

        // Empresa;
        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CompanyUser companyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        Mock<IEmailService> emailService = Fixture.CreateEmailService();
        UpdateCompany sut = CreateSut(context, user, emailService);

        CompanyInput input = company.Adapt<CompanyInput>();

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUpdatingToExistingNameOrEmail()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        user.Role = UserRoleEnum.Common;
        await Fixture.Save(context, user);

        // Empresa 1 — vai ser atualizada;
        Company company1 = CompanyMock.Create();
        await Fixture.Save(context, company1);

        CompanyUser companyUser = new()
        {
            CompanyId = company1.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator
        };

        await Fixture.Save(context, companyUser);

        // Empresa 2 — já existe no banco, vamos usar seus dados pra conflito;
        Company company2 = CompanyMock.Create();
        company2.Name = "Tmr Weon"; 
        company2.Email = "huevonazo@gmail.com"; 
        await Fixture.Save(context, company2);

        Mock<IEmailService> emailService = Fixture.CreateEmailService();
        UpdateCompany sut = CreateSut(context, user, emailService);

        CompanyInput inputNameConflict = company1.Adapt<CompanyInput>();
        inputNameConflict.Name = company2.Name; // Conflito de nome;

        CompanyInput inputEmailConflict = company1.Adapt<CompanyInput>();
        inputEmailConflict.Email = company2.Email; // Conflito de e-mail;

        // Act & Assert — nome;
        InvalidOperationException exName = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(user.UserId, inputNameConflict));
        Assert.Contains("já existe", exName.Message.ToLowerInvariant());

        // Act & Assert — email;
        InvalidOperationException exEmail = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(user.UserId, inputEmailConflict));
        Assert.Contains("já existe", exEmail.Message.ToLowerInvariant());
    }

    #region helper
    private static UpdateCompany CreateSut(Context context, User user, Mock<IEmailService> emailServiceMock)
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

        Mock<ISmsService> smsServiceMock = new();
        smsServiceMock.Setup(x => x.SendSms(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>())).ReturnsAsync("OK");

        CreateIntegrationWhatsApp createIntegrationWhatsApp = new(new IntegrationWhatsAppBaseDependencies(
            context,
            checkIfUserIsLinkedCompanyUser,
            smsServiceMock.Object
        ));

        UpdateCompany updateCompany = new(new CompanyBaseDependencies(
            context,
            envService,
            createVerification,
            inviteCompanyUser,
            updateCurrentMainCompanyUser,
            getUser,
            emailServiceMock.Object,
            checkIfUserIsLinkedCompanyUser,
            createCompanyInvoice,
            createIntegrationWhatsApp
        ));

        return updateCompany;
    }
    #endregion
}
