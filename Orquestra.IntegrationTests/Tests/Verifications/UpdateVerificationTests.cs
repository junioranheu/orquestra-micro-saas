using Orquestra.Application.UseCases.Verifications.Get;
using Orquestra.Application.UseCases.Verifications.Update;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Verifications;

public sealed class UpdateVerificationTests
{
    [Fact]
    public async Task Execute_ShouldSetUsedTrue_WhenVerificationIsValid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        UpdateVerification sut = CreateSut(context);

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
        await sut.Execute(verification.VerificationId);

        // Assert;
        Verification? updated = await context.Verifications.FindAsync(verification.VerificationId);

        Assert.NotNull(updated);
        Assert.True(updated.Used);
    }

    [Fact]
    public async Task Execute_ShouldKeepUsedTrue_WhenVerificationAlreadyUsed()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        UpdateVerification sut = CreateSut(context);

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
    private static UpdateVerification CreateSut(Context context)
    {
        GetVerification getVerification = new(context);
        UpdateVerification updateVerification = new(context, getVerification);

        return updateVerification;
    }
    #endregion
}