using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Verifications.Create;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Verifications;

public sealed class CreateVerificationTests
{
    [Fact]
    public async Task Execute_ShouldCreateVerification_WithValidData()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        CreateVerification sut = CreateSut(context);

        User user = UserMock.Create("Test User", "user@example.com", UserRoleEnum.Administrator);
        await Fixture.Save(context, user);

        // Act;
        Verification result = await sut.Execute<User>(user.UserId, VerificationTypeEnum.CompanyUser);

        // Assert;
        Assert.NotNull(result);
        Assert.Equal(user.UserId, result.EntityId);
        Assert.Equal(nameof(User), result.EntityType);
        Assert.Equal(VerificationTypeEnum.CompanyUser, result.VerificationType);
        Assert.False(result.Used);
        Assert.NotNull(result.Token);
        Assert.True(result.Token.Length > 10); // Token seguro;

        Verification dbEntity = await context.Verifications.AsNoTracking().FirstAsync();
        Assert.Equal(result.VerificationId, dbEntity.VerificationId);
    }

    [Fact]
    public async Task Execute_ShouldGenerateUniqueTokens()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        CreateVerification sut = CreateSut(context);

        Guid id = Guid.NewGuid();

        // Act;
        Verification v1 = await sut.Execute<User>(id, VerificationTypeEnum.PasswordReset);
        Verification v2 = await sut.Execute<User>(id, VerificationTypeEnum.PasswordReset);

        // Assert;
        Assert.NotEqual(v1.Token, v2.Token);
    }

    [Fact]
    public async Task Execute_ShouldAllowReference()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        CreateVerification sut = CreateSut(context);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        string reference = $"reference-{GenerateSafeToken32Bytes(urlSafe: true)}";

        // Act;
         await sut.Execute<Company>(entityId: company.CompanyId, VerificationTypeEnum.Company, reference);

        Verification? result = await context.Verifications.AsNoTracking().Where(x => x.EntityId == company.CompanyId).FirstOrDefaultAsync();

        // Assert;
        Assert.Equal(reference, result?.Reference);
        Assert.Equal(nameof(Company), result?.EntityType);
    }

    #region helpers
    private static CreateVerification CreateSut(Context context)
    {
        CreateVerification createVerificationea = new (context);

        return createVerificationea;
    }
    #endregion
}