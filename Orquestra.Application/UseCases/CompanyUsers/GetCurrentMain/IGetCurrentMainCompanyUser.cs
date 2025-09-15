using Orquestra.Application.UseCases.Companies.Shared;

namespace Orquestra.Application.UseCases.CompanyUsers.GetCurrentMain;

public interface IGetCurrentMainCompanyUser
{
    Task<(CompanyOutput? currentMainCompany, bool isUserAdm)> Execute(Guid userId);
}