using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Companies.UpdatePlanType;

public interface IUpdatePlanTypeCompany
{
    Task<CompanyInvoice?> Execute(Guid userIdAuth, Guid companyId, PlanTypeEnum planType);
}