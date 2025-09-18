using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.Auth.CreateRefreshTokenJWT;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure.Auth.Token;

namespace Orquestra.Application.UseCases.Auth.Logout
{
    public sealed class LogoutUser(
            IHttpContextAccessor httpContextAccessor,
            IJwtTokenGenerator jwtTokenGenerator,
            ICreateRefreshToken createRefreshToken
        ) : ILogoutUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
        private readonly ICreateRefreshToken _createRefreshToken = createRefreshToken;

        public async Task Execute(Guid userIdAuth)
        {
            CookieOptions cookieOptions = _jwtTokenGenerator.GetCookieOptions();
            _httpContextAccessor?.HttpContext?.Response.Cookies.Delete(SystemConsts.CookieName, cookieOptions);

            await _createRefreshToken.Update(userIdAuth, mustCheckForValidRefreshTokens: true);
        }
    }
}