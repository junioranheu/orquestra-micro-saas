using Orquestra.Application.UseCases.CompanyUsers.Shared;

namespace Orquestra.Application.UseCases.CompanyUsers.CreateRange;

public interface ICreateRangeCompanyUser
{
    Task<List<CompanyUserOutput>> Execute(Guid userId, List<CompanyUserInput> companyUsers);
}