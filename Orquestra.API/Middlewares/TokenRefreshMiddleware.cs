using Orquestra.Application.UseCases.Auth.CreateRefreshTokenJWT;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure.Auth.Token;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;

namespace Orquestra.API.Middlewares;

public sealed class TokenRefreshMiddleware(RequestDelegate next, IJwtTokenGenerator jwtTokenGenerator, IServiceScopeFactory scopeFactory)
{
    private readonly RequestDelegate _next = next;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    // Lock por userId para evitar race condition quando múltiplas requests concorrentes tentam renovar o token ao mesmo tempo;
    private static readonly ConcurrentDictionary<Guid, SemaphoreSlim> _refreshLocks = new();

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

        (bool isExpiringSoonOrHasAlreadyExpired, _, _) = _jwtTokenGenerator.IsTokenExpiringSoonOrHasAlreadyExpired(jwtToken);

        if (!isExpiringSoonOrHasAlreadyExpired)
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

        // Adquirir lock por userId — se outra request já está renovando, prosseguir sem renovar;
        SemaphoreSlim semaphore = _refreshLocks.GetOrAdd(userIdAuth, _ => new SemaphoreSlim(1, 1));

        if (!await semaphore.WaitAsync(millisecondsTimeout: 0))
        {
            // Outra request já está renovando o token deste usuário, seguir com o token atual;
            await _next(context);
            return;
        }

        try
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            ICreateRefreshToken createRefreshToken = scope.ServiceProvider.GetRequiredService<ICreateRefreshToken>();

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
        finally
        {
            semaphore.Release();
        }

        await _next(context);
    }
}