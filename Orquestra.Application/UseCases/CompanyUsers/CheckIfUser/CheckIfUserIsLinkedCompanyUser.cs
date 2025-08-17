using Orquestra.Application.UseCases.CompanyUsers.Get;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.CompanyUsers.CheckIfUser;

public sealed class CheckIfUserIsLinkedCompanyUser(IGetCompanyUser get) : ICheckIfUserIsLinkedCompanyUser
{
    private readonly IGetCompanyUser _get = get;

    public async Task<bool> Execute(Guid? companyId, Guid? userId, bool isAdmin, bool throwError = true)
    {
        if ((companyId is null || companyId == Guid.Empty) || (userId is null || userId == Guid.Empty))
        {
            throw new Exception($"Os parâmetros {nameof(companyId)} e {nameof(userId)} devem ser preenchidos corretamente.");
        }

        List<CompanyUserOutput>? result = await _get.Execute(companyId.GetValueOrDefault(), userId.GetValueOrDefault());

        if (result?.Count == 0)
        {
            if (throwError)
            {
                ThrowError(isAdmin);
            }

            return false;
        }

        CompanyUserOutput? companyUser = result?.FirstOrDefault();

        if (isAdmin)
        {
            bool checkIfIsAdmin = companyUser?.CompanyUserRole == CompanyUserRoleEnum.Administrator;

            if (!checkIfIsAdmin && throwError)
            {
                ThrowError(isAdmin);
            }

            return checkIfIsAdmin;
        }

        return true;

        static void ThrowError(bool isAdmin)
        {
            string message = isAdmin ? "Apenas o administrador da empresa pode executar esta ação." : "Apenas usuários vinculados à empresa podem executar esta ação.";
            throw new Exception(message);
        }
    }
}