using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Orquestra.Application.UseCases.Auth.RecoverPassword;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Verifications.Create;
using Orquestra.Application.UseCases.Verifications.Get;
using Orquestra.Application.UseCases.Verifications.Update;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Messaging.Publishers;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.IntegrationTests.Fixtures;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Auth;

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
            Status = true,
            RecoverPasswordQuestion = RecoverPasswordQuestionEnum.MotherName,
            RecoverPasswordAnswer = "Sandra"
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        RecoverPasswordUser sut = CreateSut(context);

        // Act;
        await sut.SendEmail(user.Email);

        // Assert: verificação criada;
        Verification? verification = await context.Verifications.AsNoTracking().FirstOrDefaultAsync(x => x.EntityId == user.UserId && x.VerificationType == VerificationTypeEnum.PasswordReset);

        Assert.NotNull(verification);
        Assert.True(verification!.ExpirationDate > GetDate());
    }

    [Fact]
    public async Task SendEmail_ShouldThrow_WhenUserDoesNotExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        RecoverPasswordUser sut = CreateSut(context);

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
            Status = true,
            RecoverPasswordQuestion = RecoverPasswordQuestionEnum.MotherName,
            RecoverPasswordAnswer = "Sandra"
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

        RecoverPasswordUser sut = CreateSut(context);

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

        RecoverPasswordUser sut = CreateSut(context);

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
            Status = true,
            RecoverPasswordQuestion = RecoverPasswordQuestionEnum.MotherName,
            RecoverPasswordAnswer = "Sandra"
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

        RecoverPasswordUser sut = CreateSut(context);

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Verify(verification.Token));
    }

    [Fact]
    public async Task Verify_ShouldThrow_WhenRecoverPasswordAnswerIsNull()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = new()
        {
            FullName = "Junior Test",
            Email = "recover@teste.com",
            Password = "123",
            Status = true,
            RecoverPasswordQuestion = RecoverPasswordQuestionEnum.MotherName,
            RecoverPasswordAnswer = string.Empty // Vazio;
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        RecoverPasswordUser sut = CreateSut(context);

        // Act & Assert;
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.SendEmail(user.Email));

        Assert.Contains("não tem nenhuma resposta de recuperação de conta.", ex.Message);
    }

    #region helpers
    private static RecoverPasswordUser CreateSut(Context context)
    {
        IWebHostEnvironment env = Fixture.CreateDevelopmentEnvironment();
        IConfiguration config = Fixture.CreateConfiguration();
        Mock<IGenericPublisher> genericPublisherMock = Fixture.CreateGenericPublisher();
        EnvService envService = new(env, config);
        GetUser getUser = new(context);
        CreateVerification createVerification = new(context);
        GetVerification getVerification = new(context);
        UpdateVerification updateVerification = new(context, getVerification);

        RecoverPasswordUser recoverPasswordUser = new(context, envService, getVerification, updateVerification, getUser, createVerification, genericPublisherMock.Object);

        return recoverPasswordUser;
    }
    #endregion
}