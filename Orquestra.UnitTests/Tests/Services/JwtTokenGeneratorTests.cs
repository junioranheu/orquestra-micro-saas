using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Auth.Models;
using Orquestra.Infrastructure.Auth.Token;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace Orquestra.UnitTests.Tests.Services;

public sealed class JwtTokenGeneratorTests
{
    [Fact]
    public void GenerateToken_ShouldReturnTokenAndRefreshToken_WithCorrectClaims()
    {
        // Arrange;
        var jwtSettings = new JwtSettings
        {
            Issuer = "testIssuer",
            Audience = "testAudience",
            TokenExpiryMinutes = 60,
            RefreshTokenExpiryMinutes = 120
        };

        IOptions<JwtSettings> mockOptions = Options.Create(jwtSettings);
        string secret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        Dictionary<string, string> inMemoryConfig = new()
        {
            { "JwtSettings:Secret", secret }
        };

        IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(inMemoryConfig!).Build();

        JwtTokenGenerator generator = new(mockOptions, config);

        Guid userIdAuth = Guid.NewGuid();
        string name = "Junior";
        string email = "junior@gmail.com";
        UserRoleEnum role = UserRoleEnum.Administrator;

        // Act;
        (string token, RefreshToken refreshToken) = generator.GenerateToken(userIdAuth, name, email, role);

        // Assert;
        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.NotNull(refreshToken);
        Assert.Equal(userIdAuth, refreshToken.UserId);

        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken jwtToken = handler.ReadJwtToken(token);

        Assert.Contains(jwtToken.Claims, c => c.Type == "nameid" && c.Value == userIdAuth.ToString());
        Assert.Contains(jwtToken.Claims, c => c.Type == "unique_name" && c.Value == name);
        Assert.Contains(jwtToken.Claims, c => c.Type == "email" && c.Value == email);

        // Role deve ter Admin e Common;
        Assert.Contains(jwtToken.Claims, c => c.Type == "role" && c.Value == "Administrator");
        Assert.Contains(jwtToken.Claims, c => c.Type == "role" && c.Value == "Common");
    }
}