using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Integrations.WhatsApp.Create;
using Orquestra.Application.UseCases.Integrations.WhatsApp.Get;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IntegrationWhatsAppController(ICreateIntegrationWhatsApp create, IGetIntegrationWhatsApp get) : BaseController<IntegrationWhatsAppController>
{
    private readonly ICreateIntegrationWhatsApp _create = create;
    private readonly IGetIntegrationWhatsApp _get = get;

    [AuthorizeFilter(modules: [ModuleEnum.IntegrationWhatsApp])]
    [HttpPost]
    public async Task<ActionResult> Create(IntegrationWhatsApp input)
    {
        if (input.CompanyId == Guid.Empty)
        {
            throw new ArgumentException($"O parâmetro {nameof(input.CompanyId)} está vazio.");
        }

        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _create.Execute(userIdAuth, companyId: input.CompanyId, input);

        return Ok(true);
    }

    [AuthorizeFilter(modules: [ModuleEnum.IntegrationWhatsApp])]
    [HttpGet]
    public async Task<ActionResult> Get(Guid? companyId)
    {
        if (companyId is null || companyId == Guid.Empty)
        {
            return NoContent();
        }

        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        IntegrationWhatsApp? output = await _get.Execute(userIdAuth, companyId.GetValueOrDefault());

        return Ok(output);
    }
}