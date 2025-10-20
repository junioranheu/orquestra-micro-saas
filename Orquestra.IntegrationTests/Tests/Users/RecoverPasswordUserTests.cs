using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Users.RecoverPassword;
using Orquestra.Application.UseCases.Verifications.Create;
using Orquestra.Application.UseCases.Verifications.Get;
using Orquestra.Application.UseCases.Verifications.Update;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Services.Email;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.IntegrationTests.Fixtures;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Users;

public sealed class RecoverPasswordUserTests
{
    [Fact]
    public async Task SendEmail_ShouldCreateVerification_WhenUserExists()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = new()
        {
            FullName = "Junior Test",
            Email = "recover@teste.com",
            Password = "123",
            Status = true
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        Dictionary<string, string>? capturedValues = null;
        Mock<IEmailService> emailServiceMock = Fixture.CreateEmailService(vals =>
        {
            capturedValues = new Dictionary<string, string>(vals);
        });

        RecoverPasswordUser sut = CreateSut(context, emailServiceMock);

        // Act;
        await sut.SendEmail(user.Email);

        // Assert: verificação criada;
        Verification? verification = await context.Verifications.AsNoTracking().FirstOrDefaultAsync(x => x.EntityId == user.UserId && x.VerificationType == VerificationTypeEnum.PasswordReset);

        Assert.NotNull(verification);
        Assert.True(verification!.ExpirationDate > GetDate());

        // Assert: e-mail enviado;
        Assert.NotNull(capturedValues);
        Assert.Equal("Junior", capturedValues!["[UserName]"]);
        Assert.Contains("/User/Verify/RecoverPassword/", capturedValues["[VerifyUrl]"]);

        emailServiceMock.Verify(x => x.SendEmail(user.Email, It.Is<string>(s => s.Contains("Resete sua senha")), It.IsAny<string>(), true, null), Times.Once);
    }

    [Fact]
    public async Task SendEmail_ShouldThrow_WhenUserDoesNotExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        Mock<IEmailService> emailServiceMock = Fixture.CreateEmailService();

        RecoverPasswordUser sut = CreateSut(context, emailServiceMock);

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.SendEmail("naoexiste@teste.com"));
    }

    [Fact]
    public async Task Verify_ShouldUpdatePassword_AndDisableVerification()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = new()
        {
            FullName = "Junior Password",
            Email = "pass@teste.com",
            Password = "SenhaAntiga",
            Status = true
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        Verification verification = new()
        {
            Token = Guid.NewGuid().ToString(),
            EntityType = nameof(User),
            EntityId = user.UserId,
            VerificationType = VerificationTypeEnum.PasswordReset,
            ExpirationDate = GetDate().AddMinutes(30),
            Status = true
        };

        await context.Verifications.AddAsync(verification);
        await context.SaveChangesAsync();

        Mock<IEmailService> emailServiceMock = Fixture.CreateEmailService();
        RecoverPasswordUser sut = CreateSut(context, emailServiceMock);

        // Act;
        await sut.Verify(verification.Token);

        // Assert: senha alterada;
        User? updatedUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == user.UserId);
        Assert.NotNull(updatedUser);
        Assert.NotEqual("SenhaAntiga", updatedUser!.Password);

        // Assert: verificação desativada;
        Verification? updatedVerification = await context.Verifications.AsNoTracking().FirstOrDefaultAsync(x => x.VerificationId == verification.VerificationId);
        Assert.True(updatedVerification!.Used);
    }

    [Fact]
    public async Task Verify_ShouldThrow_WhenTokenDoesNotExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        Mock<IEmailService> emailServiceMock = Fixture.CreateEmailService();

        RecoverPasswordUser sut = CreateSut(context, emailServiceMock);

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Verify("token_invalido"));
    }

    [Fact]
    public async Task Verify_ShouldThrow_WhenVerificationIsExpired()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = new()
        {
            FullName = "Junior Expirado",
            Email = "exp@teste.com",
            Password = "123",
            Status = true
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        Verification verification = new()
        {
            Token = Guid.NewGuid().ToString(),
            EntityType = nameof(User),
            EntityId = user.UserId,
            VerificationType = VerificationTypeEnum.PasswordReset,
            ExpirationDate = GetDate().AddMinutes(-5),
            Status = true
        };

        await context.Verifications.AddAsync(verification);
        await context.SaveChangesAsync();

        Mock<IEmailService> emailServiceMock = Fixture.CreateEmailService();
        RecoverPasswordUser sut = CreateSut(context, emailServiceMock);

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Verify(verification.Token));
    }

    #region helpers
    private static RecoverPasswordUser CreateSut(Context context, Mock<IEmailService> emailService)
    {
        IWebHostEnvironment env = Fixture.CreateDevelopmentEnvironment();
        IConfiguration config = Fixture.CreateConfiguration();
        EnvService envService = new(env, config);
        GetUser getUser = new(context);
        CreateVerification createVerification = new(context);
        GetVerification getVerification = new(context);
        UpdateVerification updateVerification = new(context, getVerification);

        RecoverPasswordUser recoverPasswordUser = new(context, envService, getVerification, updateVerification, getUser, createVerification, emailService.Object);

        return recoverPasswordUser;
    }
    #endregion
}