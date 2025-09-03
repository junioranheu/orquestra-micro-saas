using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Orquestra.Application.UseCases.Auth.CreateRefreshTokenJWT;
using Orquestra.Application.UseCases.Auth.CreateTokenJWT;
using Orquestra.Application.UseCases.Auth.GetRefreshTokenJWT;
using Orquestra.Application.UseCases.Auth.Shared;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Auth.Models;
using Orquestra.Infrastructure.Auth.Token;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Auth;

public sealed class CreateTokenTests
{
    [Fact]
    public async Task Execute_ShouldReturnUser_WhenCredentialsAreValid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        const string password = "Aea123Kappas@";
        User user = UserMock.Create(fullName: "Junior Souza", email: "junioranheu@gmail.com", password: password, role: UserRoleEnum.Common);
        await Fixture.Save(context, user);

        CreateToken sut = CreateSut(context, user);

        AuthInput input = new()
        {
            Email = user.Email,
            Password = password
        };

        // Act;
        UserOutput result = await sut.Execute(input);

        // Assert;
        Assert.NotNull(result);
        Assert.Equal(user.Email, result.Email);
        Assert.NotNull(result.RefreshTokenExpirationDate);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserNotFound()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create(fullName: "Junior Souza", email: "junioranheu@gmail.com", password: "Aea123Kappas@", role: UserRoleEnum.Common);
        await Fixture.Save(context, user);

        CreateToken sut = CreateSut(context, user);

        AuthInput input = new()
        {
            Email = "naoexiste@email.com",
            Password = "123456"
        };

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenPasswordIsInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create(fullName: "Junior Souza", email: "junioranheu@gmail.com", password: "Aea123Kappas", role: UserRoleEnum.Common);
        await Fixture.Save(context, user);

        CreateToken sut = CreateSut(context, user);

        AuthInput input = new()
        {
            Email = user.Email,
            Password = "senha_errada"
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(input));
    }

    #region helpers
    private static CreateToken CreateSut(Context context, User user)
    {
        IConfiguration config = Fixture.CreateConfiguration();

        JwtSettings jwtSettings = new()
        {
            TokenExpiryMinutes = 10,
            RefreshTokenExpiryMinutes = 30,
            Issuer = $"{SystemConsts.NameApp}.Test",
            Audience = $"{SystemConsts.NameApp}.TestAudience",
        };

        IOptions<JwtSettings> jwtOptions = Options.Create(jwtSettings);

        JwtTokenGenerator jwtTokenGenerator = new(jwtOptions, config);
        GetRefreshToken getRefreshToken = new(context);
        CreateRefreshToken createRefreshToken = new(context, jwtTokenGenerator, getRefreshToken);
        GetUser getUser = new(context);
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);

        CreateToken createToken = new(jwtTokenGenerator, createRefreshToken, getUser, httpContextAccessor);

        return createToken;
    }
    #endregion
}