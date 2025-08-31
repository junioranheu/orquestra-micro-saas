using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Users.Verify;
using Orquestra.Application.UseCases.Verifications.Get;
using Orquestra.Application.UseCases.Verifications.Update;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Users;

public sealed class VerifyUserTests
{
    [Fact]
    public async Task Execute_ShouldActivateUser_WhenTokenIsValid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        string token = GenerateSafeToken32Bytes(urlSafe: true);

        Verification verification = new()
        {
            VerificationId = Guid.NewGuid(),
            EntityId = user.UserId,
            EntityType = nameof(User),
            VerificationType = VerificationTypeEnum.User,
            Token = token,
            Used = false
        };

        await Fixture.Save(context, verification);

        VerifyUser sut = CreateSut(context);

        // Act;
        await sut.Execute(token);

        // Assert;
        User? updatedUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == user.UserId);
        Assert.NotNull(updatedUser);
        Assert.True(updatedUser.Status);

        Verification? updatedVerification = await context.Verifications.AsNoTracking().FirstOrDefaultAsync(v => v.VerificationId == verification.VerificationId);
        Assert.NotNull(updatedVerification);
        Assert.True(updatedVerification.Used);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenTokenIsInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        VerifyUser sut = CreateSut(context);

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(token: "invalid-token"));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenTokenAlreadyUsed()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        string token = GenerateSafeToken32Bytes(urlSafe: true);

        Verification verification = new()
        {
            VerificationId = Guid.NewGuid(),
            EntityId = user.UserId,
            EntityType = nameof(User),
            VerificationType = VerificationTypeEnum.User,
            Token = token,
            Used = true
        };

        await Fixture.Save(context, verification);

        VerifyUser sut = CreateSut(context);

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(token));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenTokenBelongsToOtherEntity()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        string token = GenerateSafeToken32Bytes(urlSafe: true);

        Verification verification = new()
        {
            VerificationId = Guid.NewGuid(),
            EntityId = Guid.NewGuid(),
            EntityType = nameof(User),
            VerificationType = VerificationTypeEnum.User,
            Token = token,
            Used = false
        };

        await Fixture.Save(context, verification);

        VerifyUser sut = CreateSut(context);

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(token));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenTokenIsEmpty()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        VerifyUser sut = CreateSut(context);

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(token: string.Empty));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenTokenIsNull()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        VerifyUser sut = CreateSut(context);

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(token: null!));
    }

    [Fact]
    public async Task Execute_ShouldNotActivateUser_WhenVerificationTypeIsDifferent()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        string token = GenerateSafeToken32Bytes(urlSafe: true);

        Verification verification = new()
        {
            VerificationId = Guid.NewGuid(),
            EntityId = user.UserId,
            EntityType = nameof(User),
            VerificationType = VerificationTypeEnum.CompanyUser, // Tipo errado;
            Token = token,
            Used = false
        };

        await Fixture.Save(context, verification);

        VerifyUser sut = CreateSut(context);

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(token));
    }

    [Fact]
    public async Task Execute_ShouldNotUpdateVerification_WhenUserNotFound()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        string token = GenerateSafeToken32Bytes(urlSafe: true);

        Verification verification = new()
        {
            VerificationId = Guid.NewGuid(),
            EntityId = Guid.NewGuid(), // User inexistente;
            EntityType = nameof(User),
            VerificationType = VerificationTypeEnum.User,
            Token = token,
            Used = false
        };

        await Fixture.Save(context, verification);

        VerifyUser sut = CreateSut(context);

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(token));

        // Garantir que o verification continua não usado;
        Verification? unchangedVerification = await context.Verifications.AsNoTracking().FirstOrDefaultAsync(v => v.VerificationId == verification.VerificationId);
        
        Assert.NotNull(unchangedVerification);
        Assert.False(unchangedVerification.Used);
    }

    [Fact]
    public async Task Execute_ShouldUpdateUserAndVerificationAtomically()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        string token = GenerateSafeToken32Bytes(urlSafe: true);

        Verification verification = new()
        {
            VerificationId = Guid.NewGuid(),
            EntityId = user.UserId,
            EntityType = nameof(User),
            VerificationType = VerificationTypeEnum.User,
            Token = token,
            Used = false
        };

        await Fixture.Save(context, verification);

        VerifyUser sut = CreateSut(context);

        // Act;
        await sut.Execute(token);

        // Assert;
        User? updatedUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == user.UserId);
        Verification? updatedVerification = await context.Verifications.AsNoTracking().FirstOrDefaultAsync(v => v.VerificationId == verification.VerificationId);

        Assert.NotNull(updatedUser);
        Assert.NotNull(updatedVerification);
        Assert.True(updatedUser.Status);
        Assert.True(updatedVerification.Used);
    }

    #region helpers
    private static VerifyUser CreateSut(Context context)
    {
        IGetVerification getVerification = new GetVerification(context);
        IUpdateVerification updateVerification = new UpdateVerification(context, getVerification);
        VerifyUser verifyUser = new(context, getVerification, updateVerification);

        return verifyUser;
    }
    #endregion
}