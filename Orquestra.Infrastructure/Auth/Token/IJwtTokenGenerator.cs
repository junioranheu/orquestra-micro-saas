using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using System.IdentityModel.Tokens.Jwt;

namespace Orquestra.Infrastructure.Auth.Token;

public interface IJwtTokenGenerator
{
    (string token, RefreshToken refreshToken) GenerateToken(Guid userId, string name, string email, UserRoleEnum? role);
    (bool isTokenExpiringSoonOrHasAlreadyExpired, double differenceInMinutes) IsTokenExpiringSoonOrHasAlreadyExpired(JwtSecurityToken token, int thresholdInMinutes = 0);
}