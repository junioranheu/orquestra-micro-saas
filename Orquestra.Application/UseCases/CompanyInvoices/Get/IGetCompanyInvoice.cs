using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.CompanyInvoices.Get;

public interface IGetCompanyInvoice
{
    Task<CompanyInvoice> Execute(Guid userIdAuth, Guid companyInvoiceId);
    Task<(IEnumerable<CompanyInvoice> output, int count)> Execute(PaginationInput pagination, Guid userIdAuth, Guid companyId, CompanyInvoiceSituationEnum? companyInvoiceSituationEnum);
}