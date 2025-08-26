using Microsoft.EntityFrameworkCore;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Auth.GetRefreshTokenJWT;

public sealed class GetRefreshToken(Context context) : IGetRefreshToken
{
    private readonly Context _context = context;

    public async Task<List<RefreshToken>> GetAllNotRevokedTokens(Guid userIdAuth)
    {
        IQueryable<RefreshToken> query = GetNotRevokedTokensQuery(userIdAuth);
        List<RefreshToken> output = await query.ToListAsync();

        return output;
    }

    public async Task<RefreshToken?> GetLatestNotRevokedToken(Guid userIdAuth)
    {
        IQueryable<RefreshToken> query = GetNotRevokedTokensQuery(userIdAuth);
        RefreshToken? output = await query.FirstOrDefaultAsync();

        return output;
    }

    private IQueryable<RefreshToken> GetNotRevokedTokensQuery(Guid userIdAuth)
    {
        return _context.RefreshTokens.
               AsNoTracking().
               Where(x => x.UserId == userIdAuth && x.RevokedDate == null).
               OrderByDescending(x => x.CreatedDate);
    }
}