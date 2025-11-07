using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Domain.Enums;
using System.Security.Claims;

namespace Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;

/// <summary>
/// Caso de uso responsável por verificar se um usuário está vinculado a uma empresa,
/// considerando permissões de nível de sistema (Admin/Maintainer) e permissões de nível de empresa (Admin).
/// </summary>
public sealed class CheckIfUserIsLinkedCompanyUser(IGetAllCompanyUserByCompanyId getCompanyUserByCompanyId, IHttpContextAccessor httpContextAccessor) : ICheckIfUserIsLinkedCompanyUser
{
    private readonly IGetAllCompanyUserByCompanyId _getCompanyUserByCompanyId = getCompanyUserByCompanyId;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    /// <summary>
    /// Para otimizar a performance, o resultado desta verificação é armazenado no
    /// <see cref="HttpContext.Items"/> usando uma chave única baseada nos parâmetros da chamada.
    /// Isso significa que, durante a mesma requisição HTTP, chamadas repetidas com os mesmos parâmetros
    /// retornarão o resultado previamente calculado, evitando múltiplas consultas ao banco.
    ///
    /// Esse cache é válido apenas durante a requisição atual!!! Cada nova requisição terá seu próprio
    /// cache isolado, garantindo que os dados não se misturem entre usuários ou sessões.
    /// </summary>
    public async Task<bool> Execute(Guid? companyId, Guid? userId, bool needCompanyAdmin, bool throwError = true)
    {
        if (_httpContextAccessor.HttpContext is null)
        {
            throw new InvalidOperationException("HttpContext não disponível.");
        }

        string cacheKey = $"Key_CheckIfUserIsLinkedCompanyUser_{companyId}_{userId}_{needCompanyAdmin}";

        if (_httpContextAccessor.HttpContext.Items.TryGetValue(cacheKey, out object? cached))
        {
            if (cached is bool cachedBool)
            {
                return cachedBool;
            }
        }

        bool isLinked = await InternalCheck(companyId, userId, needCompanyAdmin, throwError);

        _httpContextAccessor.HttpContext.Items[cacheKey] = isLinked;

        return isLinked;
    }

    #region extras
    /// <summary>
    /// Executa a verificação de vínculo entre um usuário e uma empresa.
    /// </summary>
    /// <param name="companyId">Identificador da empresa alvo da verificação.</param>
    /// <param name="userId">Identificador do usuário a ser validado.</param>
    /// <param name="needCompanyAdmin">
    /// Define se a verificação deve garantir que o usuário é administrador da empresa.
    /// </param>
    /// <param name="throwError">
    /// Indica se uma exceção deve ser lançada em caso de falha na validação (padrão = true).
    /// </param>
    /// <returns>
    /// Retorna <c>true</c> se o usuário estiver corretamente vinculado (e com a permissão necessária, caso exigida);
    /// caso contrário, retorna <c>false</c> (ou lança exceção se <paramref name="throwError"/> for verdadeiro).
    /// </returns>
    /// <exception cref="Exception">
    /// Lançada quando:
    /// - Os parâmetros <paramref name="companyId"/> ou <paramref name="userId"/> não forem válidos;
    /// - O usuário não estiver vinculado à empresa e <paramref name="throwError"/> for verdadeiro;
    /// - O usuário não possuir permissão de administrador quando exigido.
    /// </exception>
    private async Task<bool> InternalCheck(Guid? companyId, Guid? userId, bool needCompanyAdmin, bool throwError = true)
    {
        // #1 - Checar se o usuário autenticado é Administrador ou Suporte do SISTEMA (NÃO DA EMPRESA!);
        bool isSystemAdmin = false;

        try
        {
            ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;

            if (user is not null)
            {
                List<UserRoleEnum> roles = [.. user.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => Enum.Parse<UserRoleEnum>(x.Value))];
                isSystemAdmin = roles.Contains(UserRoleEnum.Administrator) || roles.Contains(UserRoleEnum.Maintainer);
            }
        }
        catch
        {
            isSystemAdmin = false;
        }

        // #2 - Verificar se tem algum parâmetro imprescindível vazio;
        if ((companyId is null || companyId == Guid.Empty) || (userId is null || userId == Guid.Empty))
        {
            throw new ArgumentException($"Falha interna ({nameof(CheckIfUserIsLinkedCompanyUser)}): os parâmetros <b>{nameof(companyId)}</b> e <b>{nameof(userId)}</b> devem ser preenchidos corretamente.");
        }

        // #3 - Verificar se o usuário em questão (userId) está registrado na empresa;
        List<CompanyUserOutput>? checkUsersByCompanyAndUser = await _getCompanyUserByCompanyId.Execute(companyId: companyId.GetValueOrDefault(), userId: userId.GetValueOrDefault());

        if (checkUsersByCompanyAndUser?.Count == 0)
        {
            // #2.2 - Verificação extra: verificar se a empresa em si tem algum funcionário;
            // Se não tiver, não tem sentido executar o ThrowError, porque é uma empresa nova e está recebendo seu primeiro funcionário (primerio administrador);
            // Caso tenha mais de um usuário (Count > 0), aí sim deve-se executar o ThrowError;
            List<CompanyUserOutput>? secondCheckUsersByCompanyOnly = await _getCompanyUserByCompanyId.Execute(companyId: companyId.GetValueOrDefault(), userId: null);

            if (secondCheckUsersByCompanyOnly is not null && secondCheckUsersByCompanyOnly.Count > 0)
            {
                if (throwError && !isSystemAdmin)
                {
                    ThrowError(needCompanyAdmin);
                }

                if (!isSystemAdmin)
                {
                    return false;
                }

                return true;
            }

            return true;
        }

        // #4 - Verificar se a requisição em questão necessita de permissão de Administrador da EMPRESA (NÃO DO SISTEMA!);
        // Se sim, verificar se o usuário em questão é Administrador;
        CompanyUserOutput? companyUser = checkUsersByCompanyAndUser?.FirstOrDefault();

        if (needCompanyAdmin)
        {
            bool checkIfIsAdmin = companyUser?.CompanyUserRole == CompanyUserRoleEnum.Administrator;

            if (!checkIfIsAdmin && throwError)
            {
                ThrowError(needCompanyAdmin);
            }

            return checkIfIsAdmin;
        }

        return true;
    }

    private static void ThrowError(bool needAdmin)
    {
        string message = needAdmin ? "Apenas administradores da empresa podem executar esta ação." : "Apenas usuários vinculados à empresa podem executar esta ação.";
        throw new UnauthorizedAccessException(message);
    }
    #endregion
}