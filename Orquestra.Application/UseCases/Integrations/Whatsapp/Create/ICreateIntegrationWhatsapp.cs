using Orquestra.Domain.Entities;

namespace Orquestra.Application.UseCases.Integrations.WhatsApp.Create;

public interface ICreateIntegrationWhatsApp
{
    Task Execute(Guid userIdAuth, Guid companyId, IntegrationWhatsApp? input = null);
}