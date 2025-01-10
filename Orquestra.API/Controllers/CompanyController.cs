using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Companies.Create;
using Orquestra.Application.UseCases.Companies.Shared;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController(ICreateCompany create) : BaseController<CompanyController>
{
    private readonly ICreateCompany _create = create;

    [AuthorizeFilter]
    [HttpPost]
    public async Task<ActionResult<CompanyOutput>> Create([FromForm] CompanyInput input)
    {
        Guid userId = GetUserId(throwExceptionIfNotAuth: true);

        CompanyOutput? output = await _create.Execute(userId, input);
        return output;
    }
}