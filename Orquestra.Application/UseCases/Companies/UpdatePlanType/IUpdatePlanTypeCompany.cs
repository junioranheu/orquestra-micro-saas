using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Companies.UpdatePlanType;

public interface IUpdatePlanTypeCompany
{
    Task Execute(Guid userIdAuth, Guid companyId, PlanTypeEnum planType);
}