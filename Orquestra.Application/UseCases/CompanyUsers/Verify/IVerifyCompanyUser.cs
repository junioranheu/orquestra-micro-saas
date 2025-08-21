namespace Orquestra.Application.UseCases.CompanyUsers.Verify;

public interface IVerifyCompanyUser
{
    Task Execute(string token);
}