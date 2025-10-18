using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.CompanyInvoices.Create;

public interface ICreateCompanyInvoice
{
    Task<CompanyInvoice?> Execute(Guid userIdAuth, Guid companyId, PlanTypeEnum planType, bool isCreateCompany = false);
}