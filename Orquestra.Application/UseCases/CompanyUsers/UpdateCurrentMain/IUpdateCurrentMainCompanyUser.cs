namespace Orquestra.Application.UseCases.CompanyUsers.UpdateCurrentMain;

public interface IUpdateCurrentMainCompanyUser
{
    Task Execute(Guid userIdAuth, Guid companyId);
}