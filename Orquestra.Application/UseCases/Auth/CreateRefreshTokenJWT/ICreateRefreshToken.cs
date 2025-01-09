using Orquestra.Domain.Entities;

namespace Orquestra.Application.UseCases.Auth.CreateRefreshTokenJWT;

public interface ICreateRefreshToken
{
    Task<string> RefreshToken(Guid userId);
    Task Save(RefreshToken newRefreshToken);
    Task Update(Guid userId, bool mustCheckForValidRefreshTokens);
}