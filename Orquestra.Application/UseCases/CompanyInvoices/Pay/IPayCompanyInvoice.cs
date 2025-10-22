namespace Orquestra.Application.UseCases.CompanyInvoices.Pay;

public interface IPayCompanyInvoice
{
    Task Execute(Guid userIdAuth, Guid companyInvoiceId);
}