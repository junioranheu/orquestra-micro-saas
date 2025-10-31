using Orquestra.Domain.Entities;

namespace Orquestra.Application.UseCases.Integrations.WhatsApp.Get;

public interface IGetIntegrationWhatsApp
{
    Task<IntegrationWhatsApp> Execute(Guid userIdAuth, Guid companyId);
}