using Orquestra.Application.UseCases.CompanyUsers.Verify;
using Orquestra.Application.UseCases.Verifications.Get;
using Orquestra.Application.UseCases.Verifications.Update;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.CompanyUsers;

public sealed class VerifyCompanyUserTests
{
    [Fact]
    public async Task Execute_ShouldVerifyCompanyUser_WhenTokenIsValid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        CompanyUser companyUser = new()
        {
            CompanyId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            CompanyUserRole = CompanyUserRoleEnum.Member,
            Status = false,
            UserModules = [ModuleEnum.Sales]
        };

        await Fixture.Save(context, companyUser);

        string token = GenerateSafeToken32Bytes(urlSafe: true);

        Verification verification = new()
        {
            VerificationId = Guid.NewGuid(),
            EntityType = nameof(CompanyUser),
            EntityId = companyUser.CompanyUserId,
            Token = token,
            VerificationType = VerificationTypeEnum.CompanyUser
        };

        await Fixture.Save(context, verification);

        VerifyCompanyUser sut = CreateSut(context);

        // Act;
        await sut.Execute(token);

        // Assert;
        CompanyUser? updatedUser = await context.CompanyUsers.FindAsync(companyUser.CompanyUserId);
        Assert.NotNull(updatedUser);
        Assert.True(updatedUser.Status);

        Verification? updatedVerification = await context.Verifications.FindAsync(verification.VerificationId);
        Assert.NotNull(updatedVerification);
        Assert.True(updatedVerification.Status);
    }

    [Fact]
    public async Task Execute_ShouldVerifyCompanyUser_WhenTokenIsInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        CompanyUser companyUser = new()
        {
            CompanyId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            CompanyUserRole = CompanyUserRoleEnum.Member,
            Status = false,
            UserModules = [ModuleEnum.Sales]
        };

        await Fixture.Save(context, companyUser);

        Verification verification = new()
        {
            VerificationId = Guid.NewGuid(),
            EntityType = nameof(CompanyUser),
            EntityId = companyUser.CompanyUserId,
            Token = GenerateSafeToken32Bytes(urlSafe: true),
            VerificationType = VerificationTypeEnum.CompanyUser
        };

        await Fixture.Save(context, verification);

        VerifyCompanyUser sut = CreateSut(context);

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(token: "invalid-token"));
    }

    [Fact]
    public async Task Execute_ShouldVerifyCompanyUser_WhenTokenIsInvalidBecauseOfStatusFalse()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        CompanyUser companyUser = new()
        {
            CompanyId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            CompanyUserRole = CompanyUserRoleEnum.Member,
            Status = false,
            UserModules = [ModuleEnum.Sales]
        };

        await Fixture.Save(context, companyUser);

        string token = GenerateSafeToken32Bytes(urlSafe: true);

        Verification verification = new()
        {
            VerificationId = Guid.NewGuid(),
            EntityType = nameof(CompanyUser),
            EntityId = companyUser.CompanyUserId,
            Token = token,
            VerificationType = VerificationTypeEnum.CompanyUser
        };

        await Fixture.Save(context, verification);

        verification.Status = false;
        context.Update(verification);
        await context.SaveChangesAsync();

        VerifyCompanyUser sut = CreateSut(context);

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(token: token));
    }

    [Fact]
    public async Task Execute_ShouldThrowKeyNotFound_WhenTokenDoesNotExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        VerifyCompanyUser sut = CreateSut(context);

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(token: "non-existent-token"));
    }

    [Fact]
    public async Task Execute_ShouldThrowKeyNotFound_WhenCompanyUserNotFound()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Verification verification = new()
        {
            VerificationId = Guid.NewGuid(),
            EntityType = nameof(CompanyUser),
            EntityId = Guid.NewGuid(), // Não existe user vinculado;
            Token = "orphan-token",
            VerificationType = VerificationTypeEnum.CompanyUser,
            Status = false
        };

        await Fixture.Save(context, verification);

        VerifyCompanyUser sut = CreateSut(context);

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>  sut.Execute(token: "orphan-token"));
    }

    #region helpers
    private static VerifyCompanyUser CreateSut(Context context)
    {
        GetVerification getVerification = new(context);
        UpdateVerification updateVerification = new(context, getVerification);

        VerifyCompanyUser verifyCompanyUser = new(context, getVerification, updateVerification);

        return verifyCompanyUser;
    }
    #endregion
}