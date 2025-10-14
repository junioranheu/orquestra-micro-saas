using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Application.UseCases.Shared;

namespace Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;

public interface IGetCompanyUserByCompanyId
{
    Task<List<CompanyUserOutput>?> Execute(Guid companyId, Guid? userId = null);
    Task<(IEnumerable<CompanyUserOutput> output, int count)> Execute(PaginationInput pagination, CompanyUserFilterInput input, Guid userIdAuth, Guid companyId);
}