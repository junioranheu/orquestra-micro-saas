using Orquestra.Domain.Entities;

namespace Orquestra.Application.UseCases.Auth.CreateRefreshTokenJWT;

public interface ICreateRefreshToken
{
    Task<string> RefreshToken(Guid userIdAuth);
    Task Save(RefreshToken newRefreshToken);
    Task Update(Guid userIdAuth, bool mustCheckForValidRefreshTokens);
}