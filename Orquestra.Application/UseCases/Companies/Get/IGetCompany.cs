using Orquestra.Domain.Entities;

namespace Orquestra.Application.UseCases.Companies.Get;

public interface IGetCompany
{
    Task<Company?> Execute(Guid companyId);
}