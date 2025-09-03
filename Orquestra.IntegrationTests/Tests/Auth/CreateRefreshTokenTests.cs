using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Orquestra.Application.UseCases.Auth.CreateRefreshTokenJWT;
using Orquestra.Application.UseCases.Auth.GetRefreshTokenJWT;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Auth.Models;
using Orquestra.Infrastructure.Auth.Token;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Auth;

public sealed class CreateRefreshTokenTests
{
    [Fact]
    public async Task RefreshToken_ShouldReturnNewToken_WhenUserExistsAndActive()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        // Criar refresh válido;
        RefreshToken validToken = new()
        {
            RefreshTokenId = Guid.NewGuid(),
            UserId = user.UserId,
            ExpiredDate = GetDate().AddMinutes(10)
        };

        await context.RefreshTokens.AddAsync(validToken);
        await context.SaveChangesAsync();

        CreateRefreshToken sut = CreateSut(context);

        // Act;
        (string newJwtToken, CookieOptions cookieOptions) = await sut.RefreshToken(user.UserId);

        // Assert;
        Assert.NotNull(newJwtToken);
        Assert.NotNull(cookieOptions);
        Assert.True(cookieOptions.Expires.HasValue);
    }

    [Fact]
    public async Task RefreshToken_ShouldThrow_WhenUserExistsAndActiveButHasNoValidRefreshToken()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        // Criar refresh expirado;
        RefreshToken validToken = new()
        {
            RefreshTokenId = Guid.NewGuid(),
            UserId = user.UserId,
            ExpiredDate = GetDate().AddMinutes(-10)
        };

        await context.RefreshTokens.AddAsync(validToken);
        await context.SaveChangesAsync();

        CreateRefreshToken sut = CreateSut(context);

        // Act & Assert;
        await Assert.ThrowsAsync<SecurityTokenException>(() => sut.RefreshToken(user.UserId));
    }

    [Fact]
    public async Task RefreshToken_ShouldThrow_WhenUserNotFound()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        CreateRefreshToken sut = CreateSut(context);

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.RefreshToken(Guid.NewGuid()));
    }

    [Fact]
    public async Task RefreshToken_ShouldThrow_WhenUserInactive()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        user.Status = false;
        context.Update(user);
        await context.SaveChangesAsync();

        CreateRefreshToken sut = CreateSut(context);

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.RefreshToken(user.UserId));
    }

    [Fact]
    public async Task Update_ShouldRevokeOldRefreshTokens()
    {
        // Arrange
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CreateRefreshToken sut = CreateSut(context);

        // Criar refresh válido;
        RefreshToken validToken = new()
        {
            RefreshTokenId = Guid.NewGuid(),
            UserId = user.UserId,
            ExpiredDate = GetDate().AddMinutes(10)
        };

        await context.RefreshTokens.AddAsync(validToken);
        await context.SaveChangesAsync();

        // Criar refresh expirado;
        RefreshToken expiredToken = new()
        {
            RefreshTokenId = Guid.NewGuid(),
            UserId = user.UserId,
            ExpiredDate = GetDate().AddMinutes(-10)
        };

        await context.RefreshTokens.AddAsync(expiredToken);
        await context.SaveChangesAsync();

        // Act;
        await sut.Update(user.UserId, mustCheckForValidRefreshTokens: true);

        // Assert;
        List<RefreshToken> updatedTokens = await context.RefreshTokens.AsNoTracking().Where(x => x.UserId == user.UserId).ToListAsync();

        Assert.All(updatedTokens.Where(x => x.ExpiredDate <= GetDate()), x => Assert.NotNull(x.RevokedDate));
        Assert.All(updatedTokens.Where(x => x.ExpiredDate > GetDate()), x => Assert.Null(x.RevokedDate));
    }

    #region helpers
    private static CreateRefreshToken CreateSut(Context context)
    {
        IOptions<JwtSettings> jwtOptions = Fixture.CreateJwtOptions();
        IConfiguration config = Fixture.CreateConfiguration();
        JwtTokenGenerator jwtTokenGenerator = new(jwtOptions, config);
        GetRefreshToken getRefreshToken = new(context);
        CreateRefreshToken sut = new(context, jwtTokenGenerator, getRefreshToken);

        return sut;
    }
    #endregion
}