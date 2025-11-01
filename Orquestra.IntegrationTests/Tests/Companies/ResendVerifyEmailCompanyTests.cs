using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Orquestra.Application.UseCases.Companies.Base;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.Companies.ResendVerifyEmail;
using Orquestra.Application.UseCases.CompanyInvoices.Create;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
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

public sealed class ResendVerifyEmailCompanyTests
{
    [Fact]
    public async Task Execute_ShouldResendVerification_WhenCompanyIsUnverified()
    {
        // Arrange;
        (Context context, User user, Company company) = await ArrangeCompanyWithUserAsync(status: false);

        ResendVerifyEmailCompany sut = CreateSut(context, user);

        // Act;
        await sut.Execute(user.UserId, company.CompanyId);

        // Assert;
        List<Verification> verifications = await context.Verifications.Where(x => x.EntityId == company.CompanyId && x.VerificationType == VerificationTypeEnum.Company).ToListAsync();

        Assert.NotNull(verifications);
        Assert.NotEmpty(verifications);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenCompanyAlreadyVerified()
    {
        // Arrange;
        (Context context, User user, Company company) = await ArrangeCompanyWithUserAsync(status: true);

        ResendVerifyEmailCompany sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(user.UserId, company.CompanyId));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenCompanyDoesNotExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        User user = UserMock.Create();
        await Fixture.Save(context, user);

        ResendVerifyEmailCompany sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(user.UserId, Guid.NewGuid()));
    }

    [Fact]
    public async Task Execute_ShouldDeactivateOldVerifications_WhenTheyExist()
    {
        // Arrange;
        (Context context, User user, Company company) = await ArrangeCompanyWithUserAsync(status: false);

        Verification oldVerification = new()
        {
            VerificationId = Guid.NewGuid(),
            EntityId = company.CompanyId,
            EntityType = nameof(Company),
            VerificationType = VerificationTypeEnum.Company,
            Token = Guid.NewGuid().ToString(),
            Status = true
        };

        await Fixture.Save(context, oldVerification);

        oldVerification.CreatedDate = DateTime.UtcNow.AddDays(-2); // Mais de 1 dia;
        context.Update(oldVerification);
        await context.SaveChangesAsync();

        ResendVerifyEmailCompany sut = CreateSut(context, user);

        // Act;
        await sut.Execute(user.UserId, company.CompanyId);

        // Assert;
        Verification? checkOldVerification = await context.Verifications.Where(x => x.VerificationId == oldVerification.VerificationId && x.EntityId == company.CompanyId && x.VerificationType == VerificationTypeEnum.Company).FirstOrDefaultAsync();
        List<Verification> allVerifications = await context.Verifications.Where(x => x.EntityId == company.CompanyId && x.VerificationType == VerificationTypeEnum.Company).ToListAsync();

        Assert.NotEmpty(allVerifications);
        Assert.False(checkOldVerification?.Status);
        Assert.True(allVerifications.Any(v => v.Status == false), "Pelo menos uma verificação precisa ter sido desativada");
        Assert.True(allVerifications.Any(v => v.Status == true), "Pelo menos uma verificação precisa ainda estar ativa");
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenVerificationSentLessThanOneDayAgo()
    {
        // Arrange;
        (Context context, User user, Company company) = await ArrangeCompanyWithUserAsync(status: false);

        // Cria uma verificação recente (<24h);
        Verification recentVerification = new()
        {
            VerificationId = Guid.NewGuid(),
            EntityId = company.CompanyId,
            EntityType = nameof(Company),
            VerificationType = VerificationTypeEnum.Company,
            Token = Guid.NewGuid().ToString(),
            Status = true
        };

        await Fixture.Save(context, recentVerification);

        recentVerification.CreatedDate = DateTime.UtcNow.AddHours(-12); // Menos de 1 dia;
        context.Update(recentVerification);
        await context.SaveChangesAsync();

        ResendVerifyEmailCompany sut = CreateSut(context, user);

        // Act & Assert;
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(user.UserId, company.CompanyId));
        Assert.Contains("existe uma solicitação", ex.Message);
    }

    [Fact]
    public async Task Execute_ShouldAllow_WhenVerificationOlderThanOneDay()
    {
        // Arrange;
        (Context context, User user, Company company) = await ArrangeCompanyWithUserAsync(status: false);

        // Cria uma verificação antiga (>24h);
        Verification oldVerification = new()
        {
            VerificationId = Guid.NewGuid(),
            EntityId = company.CompanyId,
            EntityType = nameof(Company),
            VerificationType = VerificationTypeEnum.Company,
            Token = Guid.NewGuid().ToString(),
            Status = true
        };

        await Fixture.Save(context, oldVerification);

        oldVerification.CreatedDate = DateTime.UtcNow.AddDays(-2); // Mais de 1 dia;
        context.Update(oldVerification);
        await context.SaveChangesAsync();

        ResendVerifyEmailCompany sut = CreateSut(context, user);

        // Act;
        await sut.Execute(user.UserId, company.CompanyId);

        // Assert;
        List<Verification> verifications = await context.Verifications.Where(x => x.EntityId == company.CompanyId && x.VerificationType == VerificationTypeEnum.Company).ToListAsync();

        Assert.NotEmpty(verifications);
        Assert.Contains(verifications, x => x.CreatedDate >= DateTime.UtcNow.AddMinutes(-1)); // Nova verificação criada agora;
    }

    #region helpers
    private static async Task<(Context context, User user, Company company)> ArrangeCompanyWithUserAsync(CompanyUserRoleEnum role = CompanyUserRoleEnum.Administrator, bool status = true)
    {
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        company.Status = status; // Forçar status;
        context.Update(company);
        await context.SaveChangesAsync();

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = role
        };

        await Fixture.Save(context, companyUser);

        return (context, user, company);
    }

    private static ResendVerifyEmailCompany CreateSut(Context context, User user, Mock<IEmailService>? emailServiceMock = null)
    {
        IConfiguration config = Fixture.CreateConfiguration();
        emailServiceMock ??= Fixture.CreateEmailService();
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
        smsServiceMock.Setup(x => x.SendSms(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>())).ReturnsAsync("OK");

        CreateIntegrationWhatsApp createIntegrationWhatsApp = new(new IntegrationWhatsAppBaseDependencies(
            context,
            checkIfUserIsLinkedCompanyUser,
            smsServiceMock.Object
        ));

        ResendVerifyEmailCompany resendVerifyEmailCompany = new(new CompanyBaseDependencies(
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

        return resendVerifyEmailCompany;
    }
    #endregion
}