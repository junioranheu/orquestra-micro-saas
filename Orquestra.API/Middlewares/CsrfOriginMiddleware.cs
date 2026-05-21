namespace Orquestra.API.Middlewares;

/// <summary>
/// Middleware de validação de Origin para proteção contra CSRF.
/// Com SameSite=None, o browser envia cookies cross-origin, permitindo que qualquer site malicioso
/// faça requests autenticados. Este middleware valida que requests mutantes (POST/PUT/DELETE)
/// venham de origens permitidas.
///
/// Regra: se o header Origin ESTÁ presente e NÃO está na lista de permitidos → bloqueia.
/// Se Origin está AUSENTE → permite (server-to-server ou same-origin, ambos seguros contra CSRF de browser).
/// </summary>
public sealed class CsrfOriginMiddleware(RequestDelegate next, IConfiguration configuration)
{
    private readonly RequestDelegate _next = next;

    /// <summary>
    /// Lista de origins permitidas.
    ///
    /// Apenas requests vindas desses frontends
    /// poderão executar operações que alteram dados.
    /// </summary>
    private readonly string[] _allowedOrigins = GetAllowedOrigins(configuration);

    /// <summary>
    /// Métodos HTTP considerados mutáveis.
    ///
    /// Esses métodos podem:
    /// - criar
    /// - editar
    /// - remover
    /// dados.
    ///
    /// Por isso precisam de proteção CSRF.
    /// </summary>
    private static readonly HashSet<string> MutateMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        "POST",
        "PUT",
        "DELETE",
        "PATCH"
    };

    /// <summary>
    /// Middleware principal.
    ///
    /// Responsabilidades:
    /// - validar Origin
    /// - bloquear requests suspeitas
    /// - permitir apenas frontends confiáveis
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        /// <summary>
        /// Apenas métodos mutáveis precisam
        /// de validação CSRF.
        ///
        /// GET normalmente não altera estado.
        /// </summary>
        if (MutateMethods.Contains(context.Request.Method))
        {
            /// <summary>
            /// Header Origin enviado pelo browser.
            ///
            /// Exemplo:
            /// https://meusite.com
            /// </summary>
            string? origin = context.Request.Headers.Origin.ToString();

            /// <summary>
            /// Se existir Origin e ela não estiver
            /// na whitelist, bloqueamos a request.
            /// </summary>
            if (!string.IsNullOrEmpty(origin) && !_allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"Messages\":[\"Origem da requisição não permitida (CSRF protection).\"]}");

                return;
            }
        }

        /// <summary>
        /// Continua pipeline normalmente.
        /// </summary>
        await _next(context);
    }

    /// <summary>
    /// Carrega lista de origins permitidas
    /// a partir do appsettings/environment.
    /// </summary>
    private static string[] GetAllowedOrigins(IConfiguration configuration)
    {
        return
        [
            configuration["Urls:Development:Frontend"] ?? string.Empty,
            configuration["Urls:Production:Frontend"] ?? string.Empty,
            configuration["Urls:Production:Frontend_2"] ?? string.Empty
        ];
    }
}