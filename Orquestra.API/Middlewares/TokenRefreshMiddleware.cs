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
        if (!context.Request.Headers.ContainsKey("Authorization"))
        {
            await _next(context);
            return;
        }

        string token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        if (!string.IsNullOrEmpty(token))
        {
            JwtSecurityToken jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

            (bool isTokenExpiringSoonOrHasAlreadyExpired, double _) = _jwtTokenGenerator.IsTokenExpiringSoonOrHasAlreadyExpired(jwtToken);

            if (isTokenExpiringSoonOrHasAlreadyExpired)
            {
                string userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value ?? string.Empty;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    throw new InvalidOperationException($"Falha ao gerar novo Token. O parâmetro userIdClaim ({userIdClaim}) está inválido");
                }

                Guid userId = Guid.Parse(userIdClaim);

                using IServiceScope scope = _scopeFactory.CreateScope();
                ICreateRefreshToken createRefreshToken = scope.ServiceProvider.GetRequiredService<ICreateRefreshToken>();

                try
                {
                    string newJwtToken = await createRefreshToken.RefreshToken(userId);

                    // Atualizar contextos (para a requisição atual);
                    context.Response.Headers.Authorization = $"Bearer {newJwtToken}";
                    context.Request.Headers.Authorization = $"Bearer {newJwtToken}";

                    // Enviar novo JWT no custom header da resposta para que o front possa interceptar e atualizar o token salvo;
                    context.Response.Headers.Append(SystemConsts.RefreshTokenJWTCustomHeader, newJwtToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    context.Response.Headers.Authorization = string.Empty;
                    context.Request.Headers.Authorization = string.Empty;
                }
            }
        }

        await _next(context);
    }
}