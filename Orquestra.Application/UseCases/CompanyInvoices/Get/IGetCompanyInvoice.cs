using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.CompanyInvoices.Get;

public interface IGetCompanyInvoice
{
    Task<CompanyInvoice> Execute(Guid userIdAuth, Guid companyId);
    Task<List<CompanyInvoice>> Execute(Guid userIdAuth, Guid companyId, CompanyInvoiceSituationEnum? companyInvoiceSituationEnum);
}