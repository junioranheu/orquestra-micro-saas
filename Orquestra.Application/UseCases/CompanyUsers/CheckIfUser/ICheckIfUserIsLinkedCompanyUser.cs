namespace Orquestra.Application.UseCases.CompanyUsers.CheckIfUser;

public interface ICheckIfUserIsLinkedCompanyUser
{
    Task<bool> Execute(Guid? companyId, Guid? userId, bool isAdmin, bool throwError = true);
}