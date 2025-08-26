using Microsoft.AspNetCore.Http;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using System.IdentityModel.Tokens.Jwt;

namespace Orquestra.Infrastructure.Auth.Token;

public interface IJwtTokenGenerator
{
    (string token, RefreshToken refreshToken, CookieOptions cookieOptions) GenerateToken(Guid userIdAuth, string name, string email, UserRoleEnum? role);
    (bool isTokenExpiringSoonOrHasAlreadyExpired, double differenceInSeconds) IsTokenExpiringSoonOrHasAlreadyExpired(JwtSecurityToken token, int thresholdInMinutes = 0);
    CookieOptions GetCookieOptions();
}