using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Integrations.Whatsapp.SendMessage;
using Orquestra.Application.UseCases.Integrations.Whatsapp.Shared;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IntegrationWhatsapp(ISendMessageWhatsapp sendMessage) : BaseController<IntegrationWhatsapp>
{
    private readonly ISendMessageWhatsapp _sendMessage = sendMessage;

    [AuthorizeFilter]
    [HttpPost]
    public async Task<ActionResult> SendMessage(IntegrationWhatsappMessageInput input)
    {
        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        await _sendMessage.Execute(userIdAuth, input);

        return Ok(true);
    }
}