using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.CompanyUsers.CreateRange;
using Orquestra.Application.UseCases.CompanyUsers.Get;
using Orquestra.Application.UseCases.CompanyUsers.Shared;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyUserController(ICreateRangeCompanyUser createRage, IGetCompanyUser get) : BaseController<CompanyUserController>
{
    private readonly ICreateRangeCompanyUser _createRage = createRage;
    private readonly IGetCompanyUser _get = get;

    [AuthorizeFilter]
    [HttpPost("CreateRange")]
    public async Task<ActionResult<CompanyUserOutput>> CreateRange([FromBody] List<CompanyUserInput> input)
    {
        if (input.Count == 0)
        {
            throw new Exception($"A lista de usuários não pode estar vazia.");
        }

        Guid userId = GetUserId(throwExceptionIfNotAuth: true);
        await _createRage.Execute(userId, input);
        return Ok();
    }

    [AuthorizeFilter]
    [HttpGet]
    public async Task<ActionResult<List<CompanyUserOutput>?>> Get(Guid companyId)
    {
        if (companyId == Guid.Empty)
        {
            throw new Exception($"O parâmetro {nameof(companyId)} não pode estar vazio.");
        }

        List<CompanyUserOutput>? output = await _get.Execute(companyId);
        return Ok(output);
    }
}