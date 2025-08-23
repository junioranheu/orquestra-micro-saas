using Microsoft.AspNetCore.Http;
using Orquestra.Domain.Entities;

namespace Orquestra.Application.UseCases.Auth.CreateRefreshTokenJWT;

public interface ICreateRefreshToken
{
    Task<(string newJwtToken, CookieOptions cookieOptions)> RefreshToken(Guid userIdAuth);
    Task Save(RefreshToken newRefreshToken);
    Task Update(Guid userIdAuth, bool mustCheckForValidRefreshTokens);
}