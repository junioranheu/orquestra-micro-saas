namespace Orquestra.Application.UseCases.CompanyUsers.UpdateCurrentMainCompany;

public interface IUpdateCurrentMainCompanyUser
{
    Task Execute(Guid userIdAuth, Guid companyId);
}