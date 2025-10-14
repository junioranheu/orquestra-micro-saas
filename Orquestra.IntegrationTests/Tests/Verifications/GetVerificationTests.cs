using Orquestra.Application.UseCases.Verifications.Get;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Verifications;

public sealed class GetVerificationTests
{
    [Fact]
    public async Task Execute_ByToken_ShouldReturnVerification_WhenValid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        GetVerification sut = CreateSut(context);

        string token = GenerateSafeToken32Bytes(urlSafe: true);

        Verification verification = new()
        {
            VerificationId = Guid.NewGuid(),
            Token = token,
            VerificationType = VerificationTypeEnum.PasswordReset,
            EntityId = Guid.NewGuid(),
            EntityType = nameof(User),
            Used = false,
            Status = true
        };

        await Fixture.Save(context, verification);

        // Act;
        Verification result = await sut.Execute(token, VerificationTypeEnum.PasswordReset);

        // Assert;
        Assert.NotNull(result);
        Assert.Equal(verification.VerificationId, result.VerificationId);
    }

    [Fact]
    public async Task Execute_ByToken_ShouldThrow_WhenNotFound()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        GetVerification sut = CreateSut(context);

        // Act & Assert;
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.Execute("invalid-token", VerificationTypeEnum.Company)
        );

        Assert.Equal(SystemConsts.Warnings.VerifyTokenInvalid, ex.Message);
    }

    [Fact]
    public async Task Execute_ByToken_ShouldThrow_WhenAlreadyUsed()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        GetVerification sut = CreateSut(context);

        string token = GenerateSafeToken32Bytes(urlSafe: true);

        Verification verification = new()
        {
            VerificationId = Guid.NewGuid(),
            Token = token,
            VerificationType = VerificationTypeEnum.PasswordReset,
            EntityId = Guid.NewGuid(),
            EntityType = nameof(User),
            Used = true,
            Status = true
        };

        await Fixture.Save(context, verification);

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.Execute(token, VerificationTypeEnum.PasswordReset)
        );
    }

    [Fact]
    public async Task Execute_ById_ShouldReturnVerification_WhenValid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        GetVerification sut = CreateSut(context);

        string token = GenerateSafeToken32Bytes(urlSafe: true);

        Verification verification = new()
        {
            VerificationId = Guid.NewGuid(),
            Token = token,
            VerificationType = VerificationTypeEnum.Company,
            EntityId = Guid.NewGuid(),
            EntityType = nameof(Company),
            Used = false,
            Status = true
        };

        await Fixture.Save(context, verification);

        // Act;
        Verification result = await sut.Execute(verification.VerificationId);

        // Assert;
        Assert.NotNull(result);
        Assert.Equal(verification.VerificationId, result.VerificationId);
    }

    [Fact]
    public async Task Execute_ById_ShouldThrow_WhenNotFound()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        GetVerification sut = CreateSut(context);

        // Act & Assert;
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(Guid.NewGuid()));

        Assert.Equal(SystemConsts.Warnings.VerifyTokenInvalid, ex.Message);
    }

    [Fact]
    public async Task Execute_ById_ShouldThrow_WhenAlreadyUsed()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        GetVerification sut = CreateSut(context);

        string token = GenerateSafeToken32Bytes(urlSafe: true);

        Verification verification = new()
        {
            VerificationId = Guid.NewGuid(),
            Token = token,
            VerificationType = VerificationTypeEnum.Company,
            EntityId = Guid.NewGuid(),
            EntityType = nameof(Company),
            Used = true,
            Status = true
        };

        await Fixture.Save(context, verification);

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(verification.VerificationId));
    }

    #region helpers
    private static GetVerification CreateSut(Context context)
    {
        GetVerification getVerification = new(context);

        return getVerification;
    }
    #endregion
}