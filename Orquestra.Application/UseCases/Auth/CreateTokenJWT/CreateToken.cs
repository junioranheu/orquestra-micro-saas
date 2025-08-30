using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.Auth.CreateRefreshTokenJWT;
using Orquestra.Application.UseCases.Auth.Shared;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Auth.Token;
using static Orquestra.Utils.Fixtures.Encrypt;

namespace Orquestra.Application.UseCases.Auth.CreateTokenJWT;

public sealed class CreateToken(
        IJwtTokenGenerator jwtTokenGenerator,
        ICreateRefreshToken createRefreshToken,
        IGetUser getUser,
        IHttpContextAccessor httpContextAccessor
    ) : ICreateToken
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly ICreateRefreshToken _createRefreshToken = createRefreshToken;
    private readonly IGetUser _getUser = getUser;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<UserOutput> Execute(AuthInput input)
    {
        (UserOutput? user, string passwordEncrypted) = await _getUser.Execute(new UserInput() { Email = input.Email });

        if (user is null)
        {
            throw new InvalidOperationException("Usuário não encontrado.");
        }

        if (!CheckPassword(password: input.Password, passwordEncrypted: passwordEncrypted))
        {
            throw new UnauthorizedAccessException("E-mail ou senha incorretos.");
        }

        (string token, RefreshToken refreshToken, CookieOptions cookieOptions) = _jwtTokenGenerator.GenerateToken(userIdAuth: user.UserId, name: user.FullName, email: user.Email, role: user.Role);

        // Revogar todos os refresh tokens antigos, caso existam;
        await _createRefreshToken.Update(userIdAuth: user.UserId, mustCheckForValidRefreshTokens: false);

        // Salvar o refresh token no banco;
        await _createRefreshToken.Save(refreshToken);

        // Escrever cookie;
        _httpContextAccessor?.HttpContext?.Response.Cookies.Append(key: SystemConsts.CookieName, value: token, cookieOptions);

        // Normalizar propriedade extra;
        user.RefreshTokenExpirationDate = cookieOptions.Expires;

        return user;
    }
}