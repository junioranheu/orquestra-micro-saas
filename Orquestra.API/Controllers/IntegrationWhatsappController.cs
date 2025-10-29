using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Integrations.Whatsapp.Create;
using Orquestra.Domain.Entities;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IntegrationWhatsappController(ICreateIntegrationWhatsapp createIntegrationWhatsapp) : BaseController<IntegrationWhatsappController>
{
    private readonly ICreateIntegrationWhatsapp _createIntegrationWhatsapp = createIntegrationWhatsapp;

    [AuthorizeFilter]
    [HttpPost]
    public async Task<ActionResult> Create(IntegrationWhatsapp input)
    {
        if (input.CompanyId == Guid.Empty)
        {
            throw new ArgumentException($"O parâmetro {nameof(input.CompanyId)} está vazio.");
        }

        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _createIntegrationWhatsapp.Execute(userIdAuth, companyId: input.CompanyId, input);

        return Ok(true);
    }
}