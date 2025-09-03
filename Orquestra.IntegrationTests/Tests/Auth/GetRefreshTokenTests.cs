using Orquestra.Application.UseCases.Auth.GetRefreshTokenJWT;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Auth;

public class GetRefreshTokenTests
{
    [Fact]
    public async Task GetAllNotRevokedTokens_WithMixedTokens_ReturnsOnlyNotRevoked()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        Guid userId = Guid.NewGuid();

        await context.RefreshTokens.AddRangeAsync(
            new RefreshToken { RefreshTokenId = Guid.NewGuid(), UserId = userId, CreatedDate = GetDate(), RevokedDate = null },
            new RefreshToken { RefreshTokenId = Guid.NewGuid(), UserId = userId, CreatedDate = GetDate().AddMinutes(-10), RevokedDate = null },
            new RefreshToken { RefreshTokenId = Guid.NewGuid(), UserId = userId, CreatedDate = GetDate().AddMinutes(-20), RevokedDate = GetDate() }
        );

        await context.SaveChangesAsync();

        GetRefreshToken sut = CreateSut(context);

        // Act;
        List<RefreshToken> result = await sut.GetAllNotRevokedTokens(userId);

        // Assert;
        Assert.Equal(2, result.Count);
        Assert.All(result, x => Assert.Null(x.RevokedDate));
    }

    [Fact]
    public async Task GetLatestNotRevokedToken_WithMultipleTokens_ReturnsMostRecent()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        Guid userId = Guid.NewGuid();

        RefreshToken olderToken = new()
        {
            RefreshTokenId = Guid.NewGuid(),
            UserId = userId,
            CreatedDate = GetDate().AddMinutes(-10),
            RevokedDate = null
        };

        RefreshToken latestToken = new()
        {
            RefreshTokenId = Guid.NewGuid(),
            UserId = userId,
            CreatedDate = GetDate(),
            RevokedDate = null
        };

        context.RefreshTokens.AddRange(olderToken, latestToken);
        await context.SaveChangesAsync();

        GetRefreshToken sut = CreateSut(context);

        // Act;
        RefreshToken? result = await sut.GetLatestNotRevokedToken(userId);

        // Assert;
        Assert.NotNull(result);
        Assert.Equal(latestToken.RefreshTokenId, result.RefreshTokenId);
    }

    [Fact]
    public async Task GetAllNotRevokedTokens_WithDifferentUserId_ReturnsEmptyList()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        Guid userId = Guid.NewGuid();
        Guid anotherUserId = Guid.NewGuid();

        await context.RefreshTokens.AddRangeAsync(
             new RefreshToken { RefreshTokenId = Guid.NewGuid(), UserId = anotherUserId, CreatedDate = GetDate(), RevokedDate = null },
             new RefreshToken { RefreshTokenId = Guid.NewGuid(), UserId = anotherUserId, CreatedDate = GetDate().AddMinutes(-5), RevokedDate = null }
         );

        await context.SaveChangesAsync();

        GetRefreshToken sut = CreateSut(context);

        // Act;
        List<RefreshToken> result = await sut.GetAllNotRevokedTokens(userId);

        // Assert;
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetLatestNotRevokedToken_WithDifferentUserId_ReturnsNull()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        Guid userId = Guid.NewGuid();
        Guid anotherUserId = Guid.NewGuid();

        await context.RefreshTokens.AddAsync(
            new RefreshToken { RefreshTokenId = Guid.NewGuid(), UserId = anotherUserId, CreatedDate = GetDate(), RevokedDate = null }
        );

        await context.SaveChangesAsync();

        GetRefreshToken sut = CreateSut(context);

        // Act;
        RefreshToken? result = await sut.GetLatestNotRevokedToken(userId);

        // Assert;
        Assert.Null(result);
    }

    #region helper
    private static GetRefreshToken CreateSut(Context context)
    {
        GetRefreshToken getRefreshToken = new(context);

        return getRefreshToken;
    }
    #endregion
}