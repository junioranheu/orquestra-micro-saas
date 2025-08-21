using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.CompanyUsers.CreateRange;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.CompanyUsers.Shared;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyUserController(ICreateRangeCompanyUser createRange, IGetCompanyUserByCompanyId getCompanyUserByCompanyId) :
    BaseController<CompanyUserController>
{
    private readonly ICreateRangeCompanyUser _createRange = createRange;
    private readonly IGetCompanyUserByCompanyId _getCompanyUserByCompanyId = getCompanyUserByCompanyId;

    [AuthorizeFilter]
    [HttpPost("CreateRange")]
    public async Task<ActionResult> CreateRange([FromBody] List<CompanyUserInput> input)
    {
        if (input.Count == 0)
        {
            throw new Exception($"A lista de usuários não pode estar vazia.");
        }

        Guid userIdAuth = GetUserIdAuth(throwExceptionIfNotAuth: true);
        List<CompanyUserOutput> output = await _createRange.Execute(userIdAuth, input);
        // TO DO: Enviar e-mail para cada um dos usuários para eles aceitarem e mudar o status do IsAccountVerified;

        return Ok(output);
    }

    [AuthorizeFilter]
    [HttpGet("GetAllByCompanyId")]
    public async Task<ActionResult> GetAllByCompanyId(Guid companyId)
    {
        if (companyId == Guid.Empty)
        {
            throw new Exception($"O parâmetro {nameof(companyId)} não pode estar vazio.");
        }

        List<CompanyUserOutput>? output = await _getCompanyUserByCompanyId.Execute(companyId);
        return Ok(output);
    }
}