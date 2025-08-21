using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Auth.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Infrastructure.Auth.Token;

public sealed class JwtTokenGenerator(IOptions<JwtSettings> jwtOptions, IConfiguration config) : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;
    private readonly string _secret = config["JwtSettings:Secret"] ?? throw new InvalidOperationException("JWT Secret não foi configurada no servidor!");

    public (string token, RefreshToken refreshToken) GenerateToken(Guid userIdAuth, string name, string email, UserRoleEnum? role)
    {
        JwtSecurityTokenHandler tokenHandler = new();

        SigningCredentials signingCredentials = new(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret ?? string.Empty)),
            algorithm: SecurityAlgorithms.HmacSha256Signature
        );

        List<Claim> claimList = [
            new(ClaimTypes.NameIdentifier, userIdAuth.ToString()),
            new(ClaimTypes.Name, name),
            new(ClaimTypes.Email, email)
        ];

        if (role is not null && role.HasValue)
        {
            Claim roleClaim = new(ClaimTypes.Role, role.Value.ToString());
            claimList.Add(roleClaim);

            bool alreadyHasCommon = role == UserRoleEnum.Common;

            if (!alreadyHasCommon)
            {
                Claim roleComum = new(ClaimTypes.Role, UserRoleEnum.Common.ToString());
                claimList.Add(roleComum);
            }
        }

        ClaimsIdentity claims = new(claimList);

        DateTime date = GetDate();

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Issuer = _jwtSettings.Issuer,
            IssuedAt = date,
            Audience = _jwtSettings.Audience,
            NotBefore = date,
            Expires = date.AddMinutes(_jwtSettings.TokenExpiryMinutes),
            Subject = claims,
            SigningCredentials = signingCredentials
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        string jwt = tokenHandler.WriteToken(token);
        RefreshToken refreshToken = GenerateRefreshToken(userIdAuth);

        return (jwt, refreshToken);
    }

    #region extras
    private RefreshToken GenerateRefreshToken(Guid userIdAuth)
    {
        string token = GenerateRefreshTokenStr();

        RefreshToken refreshToken = new()
        {
            Token = token,
            UserId = userIdAuth,
            CreatedDate = GetDate(),
            ExpiredDate = GetDate().AddMinutes(_jwtSettings.RefreshTokenExpiryMinutes),
            RevokedDate = null
        };

        return refreshToken;
    }

    private static string GenerateRefreshTokenStr()
    {
        var random = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(random);
        var refreshToken = Convert.ToBase64String(random);

        return refreshToken;
    }

    public (bool isTokenExpiringSoonOrHasAlreadyExpired, double differenceInMinutes) IsTokenExpiringSoonOrHasAlreadyExpired(JwtSecurityToken token, int thresholdInMinutes = 0)
    {
        DateTime date = GetDate();
        DateTime dateThreshold = date.AddMinutes(thresholdInMinutes);

        double differenceInMinutes = (token.ValidTo - dateThreshold).TotalMinutes;
        bool isTokenExpiringSoonOrHasAlreadyExpired = differenceInMinutes <= 0;

        return (isTokenExpiringSoonOrHasAlreadyExpired, differenceInMinutes);
    }
    #endregion
}