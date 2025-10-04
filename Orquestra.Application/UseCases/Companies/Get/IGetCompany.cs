using Orquestra.Application.UseCases.Companies.Shared;

namespace Orquestra.Application.UseCases.Companies.Get;

public interface IGetCompany
{
    Task<CompanyOutput> Execute(Guid userIdAuth, Guid companyId, bool throwIfStatusFalse = true);
    Task<List<CompanyOutput>?> Execute(bool onlyStatusTrue);
    Task<List<CompanyOutput>?> Execute(Guid userId, bool onlyStatusTrue);
}