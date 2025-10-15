namespace Orquestra.Application.UseCases.CompanyUsers.Delete;

public interface IDeleteCompanyUser
{
    Task Execute(Guid userIdAuth, Guid companyId, Guid userId);
}