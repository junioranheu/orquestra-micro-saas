using Orquestra.Domain.Entities;

namespace Orquestra.Application.UseCases.Integrations.Whatsapp.Create;

public interface ICreateIntegrationWhatsapp
{
    Task Execute(Guid userIdAuth, Guid companyId, IntegrationWhatsapp? input = null);
}