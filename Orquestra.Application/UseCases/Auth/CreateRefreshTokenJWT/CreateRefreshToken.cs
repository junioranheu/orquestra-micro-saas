using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Auth.Token;
using Orquestra.Infrastructure.Data;
using System.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Auth.CreateRefreshTokenJWT;

public sealed class CreateRefreshToken(Context context, IJwtTokenGenerator jwtTokenGenerator) : ICreateRefreshToken
{
    private readonly Context _context = context;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;

    public async Task<string> RefreshToken(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new Exception($"Parâmetro {nameof(userId)} está vazio em {nameof(RefreshToken)}");
        }

        User user = await GetUser(userId);

        // Gere novo JWT e refresh token;
        (string newJwtToken, RefreshToken _) = _jwtTokenGenerator.GenerateToken(userId: user.UserId, name: user.FullName, email: user.Email, role: user.Role);

        // Revogue os antigos refresh tokens inválidos;
        await Update(userId, mustCheckForValidRefreshTokens: true);

        return newJwtToken;
    }

    public async Task Save(RefreshToken newRefreshToken)
    {
        await _context.RefreshTokens.AddAsync(newRefreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Guid userId, bool mustCheckForValidRefreshTokens)
    {
        List<RefreshToken> oldRefreshTokens = await GetOldRefreshTokensAndCheckIfRefreshTokenIsValid(userId, mustCheckForValidRefreshTokens);

        if (oldRefreshTokens.Count == 0)
        {
            return;
        }

        List<Guid> oldRefreshTokenIds = [.. oldRefreshTokens.Select(y => y.RefreshTokenId)];

        string sql = "UPDATE RefreshTokens SET Status = @Status, RevokedDate = @RevokedDate WHERE RefreshTokenId IN (@OldRefreshTokenIds)";

        var parameters = new[]
        {
            new MySqlParameter("@Status", MySqlDbType.Bit) { Value = false },
            new MySqlParameter("@RevokedDate", MySqlDbType.DateTime) { Value = GetDate() },
            new MySqlParameter("@OldRefreshTokenIds", MySqlDbType.String) { Value = string.Join(",", oldRefreshTokenIds) }
        };

        await _context.Database.ExecuteSqlRawAsync(sql, parameters);

        #region obsoleto_dotNet9
        //await _context.RefreshTokens.
        //AsNoTracking().
        //Where(x => oldRefreshTokenIds.Contains(x.RefreshTokenId)).
        //ExecuteUpdateAsync(x => x.
        //    SetProperty(prop => prop.Status, false).
        //    SetProperty(prop => prop.RevokedDate, GetDate())
        //);
        #endregion
    }

    #region extras
    private async Task<List<RefreshToken>> GetOldRefreshTokensAndCheckIfRefreshTokenIsValid(Guid userId, bool mustCheckForValidRefreshTokens)
    {
        List<RefreshToken> oldRefreshTokens = await _context.RefreshTokens.
                                              AsNoTracking().
                                              Where(x =>
                                                 x.UserId == userId &&
                                                 x.Status == true
                                              ).
                                              OrderByDescending(x => x.CreatedDate).
                                              ToListAsync();

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

    private async Task<User> GetUser(Guid userId)
    {
        User? user = await _context.Users.
                     AsNoTracking().
                     Where(x => x.UserId == userId).
                     FirstOrDefaultAsync() ?? throw new Exception($"Usuário {userId} não encontrado");

        if (!user.Status)
        {
            throw new Exception($"O usuário {user.Email} ({userId}) está desativado");
        }

        return user;
    }
    #endregion
}