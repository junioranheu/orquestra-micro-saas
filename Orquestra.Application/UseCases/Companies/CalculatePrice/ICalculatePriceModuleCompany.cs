using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Companies.CalculatePrice;

public interface ICalculatePriceModuleCompany
{
    Task<List<CalculatePriceModuleCompanyOutput>> Execute(Guid userIdAuth, Guid companyId, ModuleEnum[]? modules);
}