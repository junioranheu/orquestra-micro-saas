using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Integrations.WhatsApp.Create;
using Orquestra.Domain.Entities;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IntegrationWhatsAppController(ICreateIntegrationWhatsApp createIntegrationWhatsApp) : BaseController<IntegrationWhatsAppController>
{
    private readonly ICreateIntegrationWhatsApp _createIntegrationWhatsApp = createIntegrationWhatsApp;

    [AuthorizeFilter]
    [HttpPost]
    public async Task<ActionResult> Create(IntegrationWhatsApp input)
    {
        if (input.CompanyId == Guid.Empty)
        {
            throw new ArgumentException($"O parâmetro {nameof(input.CompanyId)} está vazio.");
        }

        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _createIntegrationWhatsApp.Execute(userIdAuth, companyId: input.CompanyId, input);

        return Ok(true);
    }
}