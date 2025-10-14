using Orquestra.Application.UseCases.Auth.CreateRefreshTokenJWT;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure.Auth.Token;
using System.IdentityModel.Tokens.Jwt;

namespace Orquestra.API.Middlewares;

public sealed class TokenRefreshMiddleware(RequestDelegate next, IJwtTokenGenerator jwtTokenGenerator, IServiceScopeFactory scopeFactory)
{
    private readonly RequestDelegate _next = next;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Cookies.TryGetValue(SystemConsts.Cookies.Auth, out string? token) || string.IsNullOrEmpty(token))
        {
            await _next(context);
            return;
        }

        JwtSecurityToken jwtToken;

        try
        {
            jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        }
        catch
        {
            // Cookie inválido: limpa e segue;
            context.Response.Cookies.Delete(SystemConsts.Cookies.Auth);
            await _next(context);
            return;
        }

        (bool isExpiringSoon, _, _) = _jwtTokenGenerator.IsTokenExpiringSoonOrHasAlreadyExpired(jwtToken);

        if (!isExpiringSoon)
        {
            await _next(context);
            return;
        }

        string? userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            context.Response.Cookies.Delete(SystemConsts.Cookies.Auth);
            await _next(context);
            return;
        }

        Guid userIdAuth = Guid.Parse(userIdClaim);
        using IServiceScope scope = _scopeFactory.CreateScope();
        ICreateRefreshToken createRefreshToken = scope.ServiceProvider.GetRequiredService<ICreateRefreshToken>();

        try
        {
            (string newJwtToken, CookieOptions cookieOptions) = await createRefreshToken.RefreshToken(userIdAuth);

            // Escreve cookie pra próxima requisição do browser com o novo refresh token;
            context.Response.Cookies.Append(key: SystemConsts.Cookies.Auth, value: newJwtToken, cookieOptions);

            // Guarda o token renovado no Items (expira depois dessa request) para o JwtBearer usar nesta mesma request;
            context.Items[SystemConsts.Cookies.Refresh] = newJwtToken;
        }
        catch (Exception)
        {
            context.Response.Cookies.Delete(SystemConsts.Cookies.Auth);
        }

        await _next(context);
    }
}