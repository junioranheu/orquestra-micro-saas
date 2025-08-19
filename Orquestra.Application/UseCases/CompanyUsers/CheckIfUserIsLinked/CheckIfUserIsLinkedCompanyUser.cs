using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;

public sealed class CheckIfUserIsLinkedCompanyUser(IGetCompanyUserByCompanyId getCompanyUserByCompanyId) : ICheckIfUserIsLinkedCompanyUser
{
    private readonly IGetCompanyUserByCompanyId _getCompanyUserByCompanyId = getCompanyUserByCompanyId;

    public async Task<bool> Execute(Guid? companyId, Guid? userId, bool needAdmin, bool throwError = true)
    {
        if ((companyId is null || companyId == Guid.Empty) || (userId is null || userId == Guid.Empty))
        {
            throw new Exception($"Os parâmetros {nameof(companyId)} e {nameof(userId)} devem ser preenchidos corretamente.");
        }

        List<CompanyUserOutput>? result = await _getCompanyUserByCompanyId.Execute(companyId: companyId.GetValueOrDefault(), userId: userId.GetValueOrDefault());

        if (result?.Count == 0)
        {
            if (throwError)
            {
                ThrowError(needAdmin);
            }

            return false;
        }

        CompanyUserOutput? companyUser = result?.FirstOrDefault();

        if (needAdmin)
        {
            bool checkIfIsAdmin = companyUser?.CompanyUserRole == CompanyUserRoleEnum.Administrator || companyUser?.CompanyUserRole == CompanyUserRoleEnum.Owner;

            if (!checkIfIsAdmin && throwError)
            {
                ThrowError(needAdmin);
            }

            return checkIfIsAdmin;
        }

        return true;

        static void ThrowError(bool needAdmin)
        {
            string message = needAdmin ? "Apenas o administrador da empresa pode executar esta ação." : "Apenas usuários vinculados à empresa podem executar esta ação.";
            throw new Exception(message);
        }
    }
}