using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Domain.Enums;
using System.Security.Claims;

namespace Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;

public sealed class CheckIfUserIsLinkedCompanyUser(IGetCompanyUserByCompanyId getCompanyUserByCompanyId, IHttpContextAccessor httpContextAccessor) : ICheckIfUserIsLinkedCompanyUser
{
    private readonly IGetCompanyUserByCompanyId _getCompanyUserByCompanyId = getCompanyUserByCompanyId;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<bool> Execute(Guid? companyId, Guid? userId, bool needCompanyAdmin, bool throwError = true)
    {
        // #1 - Checar se o usuário autenticado é Administrador ou Suporte do SISTEMA (NÃO DA EMPRESA!);
        bool isSystemAdmin = false;

        try
        {
            ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;

            if (user is not null)
            {
                List<UserRoleEnum> roles = [.. user.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => Enum.Parse<UserRoleEnum>(x.Value))];
                isSystemAdmin = roles.Contains(UserRoleEnum.Admin) || roles.Contains(UserRoleEnum.Maintainer);
            }
        }
        catch
        {
            isSystemAdmin = false;
        }

        // #2 - Verificar se tem algum parâmetro super importante vazio;
        if ((companyId is null || companyId == Guid.Empty) || (userId is null || userId == Guid.Empty))
        {
            throw new Exception($"Os parâmetros {nameof(companyId)} e {nameof(userId)} devem ser preenchidos corretamente.");
        }

        // #3 - Verificar se o usuário em questão (userId) está registrado na empresa;
        List<CompanyUserOutput>? checkUsersByCompanyAndUser = await _getCompanyUserByCompanyId.Execute(companyId: companyId.GetValueOrDefault(), userId: userId.GetValueOrDefault());

        if (checkUsersByCompanyAndUser?.Count == 0)
        {
            // #2.2 - Verificação extra: verificar se a empresa em si tem algum funcionário;
            // Se não tiver, não tem sentido executar o ThrowError, porque é uma empresa nova e está recebendo seu primeiro funcionário (Dono);
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

        // #4 - Verificar se a requisição em questão necessita de permissão de Administrador (ou Dono) da EMPRESA (NÃO DO SISTEMA!);
        // Se sim, verificar se o usuário em questão é Administrador (ou Dono);
        CompanyUserOutput? companyUser = checkUsersByCompanyAndUser?.FirstOrDefault();

        if (needCompanyAdmin)
        {
            bool checkIfIsAdmin = companyUser?.CompanyUserRole == CompanyUserRoleEnum.Administrator || companyUser?.CompanyUserRole == CompanyUserRoleEnum.Owner;

            if (!checkIfIsAdmin && throwError)
            {
                ThrowError(needCompanyAdmin);
            }

            return checkIfIsAdmin;
        }

        return true;
    }

    #region extras
    private static void ThrowError(bool needAdmin)
    {
        string message = needAdmin ? "Apenas administradores da empresa podem executar esta ação." : "Apenas usuários vinculados à empresa podem executar esta ação.";
        throw new Exception(message);
    }
    #endregion
}