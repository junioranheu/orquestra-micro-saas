using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.Verify;
using Orquestra.Application.UseCases.Verifications.Get;
using Orquestra.Application.UseCases.Verifications.Update;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Companies;

public sealed class VerifyCompanyTests
{
    [Fact]
    public async Task Execute_ShouldVerifyCompany_WhenValidToken()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        company.Status = false; // Forçar status false;
        context.Update(company);
        await context.SaveChangesAsync();

        Verification verification = new()
        {
            VerificationId = Guid.NewGuid(),
            Token = GenerateSafeToken32Bytes(urlSafe: true),
            VerificationType = VerificationTypeEnum.Company,
            EntityId = company.CompanyId,
            EntityType = nameof(Company),
            Used = false,
            Status = true
        };

        await Fixture.Save(context, verification);

        VerifyCompany sut = CreateSut(context);

        // Act;
        await sut.Execute(verification.Token);

        // Assert;
        Company? updated = await context.Companies.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == company.CompanyId);
        Assert.NotNull(updated);
        Assert.True(updated!.Status);

        Verification? updatedVerification = await context.Verifications.AsNoTracking().FirstOrDefaultAsync(v => v.VerificationId == verification.VerificationId);
        Assert.NotNull(updatedVerification);
        Assert.True(updatedVerification!.Used);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenTokenDoesNotBelongToCompany()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        // Verification exists but points to a different (non-existent) company id;
        Verification verification = new()
        {
            VerificationId = Guid.NewGuid(),
            Token = GenerateSafeToken32Bytes(urlSafe: true),
            VerificationType = VerificationTypeEnum.Company,
            EntityId = Guid.NewGuid(),
            EntityType = nameof(Company),
            Used = false,
            Status = true
        };

        await Fixture.Save(context, verification);

        VerifyCompany sut = CreateSut(context);

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(verification.Token));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenVerificationAlreadyUsed()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        // Verification already used -> GetVerification will throw;
        Verification verification = new()
        {
            VerificationId = Guid.NewGuid(),
            Token = GenerateSafeToken32Bytes(urlSafe: true),
            VerificationType = VerificationTypeEnum.Company,
            EntityId = company.CompanyId,
            EntityType = nameof(Company),
            Used = true,
            Status = true
        };

        await Fixture.Save(context, verification);

        VerifyCompany sut = CreateSut(context);

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(verification.Token));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenTokenNotFound()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        VerifyCompany sut = CreateSut(context);

        string notExistingToken = "this-token-does-not-exist";

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(notExistingToken));
    }

    #region helpers
    private static VerifyCompany CreateSut(Context context)
    {
        IGetVerification getVerification = new GetVerification(context);
        IUpdateVerification updateVerification = new UpdateVerification(context, getVerification);

        VerifyCompany verifyCompany = new(context, getVerification, updateVerification);

        return verifyCompany;
    }
    #endregion
}