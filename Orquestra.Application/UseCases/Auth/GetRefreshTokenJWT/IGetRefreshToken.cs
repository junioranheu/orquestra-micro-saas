using Orquestra.Domain.Entities;

namespace Orquestra.Application.UseCases.Auth.GetRefreshTokenJWT;

public interface IGetRefreshToken
{
    Task<List<RefreshToken>> GetAllNotRevokedTokens(Guid userIdAuth);
    Task<RefreshToken?> GetLatestNotRevokedToken(Guid userIdAuth);
}