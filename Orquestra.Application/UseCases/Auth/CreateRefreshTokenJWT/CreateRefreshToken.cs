using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Orquestra.Application.UseCases.Auth.GetRefreshTokenJWT;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Auth.Token;
using Orquestra.Infrastructure.Data;
using System.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Auth.CreateRefreshTokenJWT;

public sealed class CreateRefreshToken(Context context, IJwtTokenGenerator jwtTokenGenerator, IGetRefreshToken getRefreshToken) : ICreateRefreshToken
{
    private readonly Context _context = context;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IGetRefreshToken _getRefreshToken = getRefreshToken;

    public async Task<(string newJwtToken, CookieOptions cookieOptions)> RefreshToken(Guid userIdAuth)
    {
        if (userIdAuth == Guid.Empty)
        {
            throw new ArgumentException($"Parâmetro {nameof(userIdAuth)} está vazio em {nameof(RefreshToken)}.");
        }

        User user = await GetUser(userIdAuth);

        // Gere novo JWT e refresh token;
        (string newJwtToken, RefreshToken refreshToken, CookieOptions cookieOptions) = _jwtTokenGenerator.GenerateToken(userIdAuth: user.UserId, name: user.FullName, email: user.Email, role: user.Role);

        // Revogue os antigos refresh tokens inválidos;
        await Update(userIdAuth, mustCheckForValidRefreshTokens: true);

        // Persistir o novo refresh token no banco para que a sessão seja renovada junto com o JWT;
        await Save(refreshToken);

        return (newJwtToken, cookieOptions);
    }

    public async Task Save(RefreshToken newRefreshToken)
    {
        await _context.RefreshTokens.AddAsync(newRefreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Guid userIdAuth, bool mustCheckForValidRefreshTokens)
    {
        List<RefreshToken> oldRefreshTokens = await GetOldRefreshTokensAndCheckIfRefreshTokenIsValid(userIdAuth, mustCheckForValidRefreshTokens);

        if (oldRefreshTokens.Count == 0)
        {
            return;
        }

        List<Guid> oldRefreshTokenIds = [.. oldRefreshTokens.Select(y => y.RefreshTokenId)];

        #region obsoleto_raw_sql
        //string sql = "UPDATE RefreshTokens SET Status = @Status, RevokedDate = @RevokedDate WHERE RefreshTokenId IN (@OldRefreshTokenIds)";

        //var parameters = new[]
        //{
        //    new NpgsqlParameter("@Status", NpgsqlDbType.Boolean) { Value = false },
        //    new NpgsqlParameter("@RevokedDate", NpgsqlDbType.Timestamp) { Value = GetDate() },
        //    new NpgsqlParameter("@OldRefreshTokenIds", NpgsqlDbType.Text) { Value = string.Join(",", oldRefreshTokenIds) }
        //};

        //await _context.Database.ExecuteSqlRawAsync(sql, parameters);
        #endregion

        #region teste_de_integracao
        // Foi necessário criar esse IF para os testes de integração porque aparentemente ele não entende o ExecuteUpdateAsync;
        if (_context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
        {
            List<RefreshToken> tokens = await _context.RefreshTokens.Where(x => oldRefreshTokenIds.Contains(x.RefreshTokenId)).ToListAsync();

            foreach (var t in tokens)
            {
                t.RevokedDate = GetDate();
            }

            await _context.SaveChangesAsync();
            return;
        }
        #endregion

        await _context.RefreshTokens.
            // AsNoTracking(). // Propositalmente sem AsNoTracking;
            Where(x => oldRefreshTokenIds.Contains(x.RefreshTokenId)).
            ExecuteUpdateAsync(x => x.
                SetProperty(prop => prop.RevokedDate, GetDate())
            );
    }

    #region extras
    private async Task<List<RefreshToken>> GetOldRefreshTokensAndCheckIfRefreshTokenIsValid(Guid userIdAuth, bool mustCheckForValidRefreshTokens)
    {
        List<RefreshToken> oldRefreshTokens = await _getRefreshToken.GetAllNotRevokedTokens(userIdAuth);

        // Se mustCheckForValidRefreshTokens for false, significa que deve ser retornado todos os registros, sem validações posteriores;
        if (!mustCheckForValidRefreshTokens)
        {
            return oldRefreshTokens;
        }

        DateTime date = GetDate();
        List<RefreshToken> validRefreshTokens = [.. oldRefreshTokens.Where(x => x.ExpiredDate > date)];
        List<RefreshToken> invalidRefreshTokens = [.. oldRefreshTokens.Where(x => x.ExpiredDate <= date)];

        if (validRefreshTokens is null || validRefreshTokens.Count == 0)
        {
            throw new SecurityTokenException("Refresh token inválido, autentique-se novamente");
        }

        return invalidRefreshTokens;
    }

    private async Task<User> GetUser(Guid userIdAuth)
    {
        var user = await _context.Users.
                   AsNoTracking().
                   Where(x => x.UserId == userIdAuth).
                   FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundUser);

        if (!user.Status)
        {
            throw new InvalidOperationException($"O usuário {user.Email} ({userIdAuth}) está desativado.");
        }

        return user;
    }
    #endregion
}