using Orquestra.Domain.Entities;

namespace Orquestra.Application.UseCases.CompanyUsers.CreateRange;

public interface ICreateRangeCompanyUser
{
    Task Execute(List<CompanyUser> companyUsers);
}