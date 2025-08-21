namespace Orquestra.Application.UseCases.Companies.Verify;

public interface IVerifyCompany
{
    Task Execute(string token);
}