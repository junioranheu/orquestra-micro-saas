namespace Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;

public interface ICheckIfUserIsLinkedCompanyUser
{
    Task<bool> Execute(Guid? companyId, Guid? userId, bool needCompanyAdmin, bool throwError = true);
}