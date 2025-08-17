using Orquestra.Application.UseCases.CompanyUsers.Shared;

namespace Orquestra.Application.UseCases.CompanyUsers.CreateRange;

public interface ICreateRangeCompanyUser
{
    Task Execute(List<CompanyUserInput> companyUsers);
}