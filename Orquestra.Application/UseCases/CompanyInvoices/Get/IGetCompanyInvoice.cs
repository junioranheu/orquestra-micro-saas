using Orquestra.Domain.Entities;

namespace Orquestra.Application.UseCases.CompanyInvoices.Get;

public interface IGetCompanyInvoice
{
    Task<CompanyInvoice> Execute(Guid userIdAuth, Guid companyId);
}