using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Companies.Create;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.Companies.Shared;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController(ICreateCompany create, IGetCompany get) : BaseController<CompanyController>
{
    private readonly ICreateCompany _create = create;
    private readonly IGetCompany _get = get;

    [AuthorizeFilter]
    [HttpPost]
    public async Task<ActionResult<CompanyOutput>> Create([FromForm] CompanyInput input)
    {
        Guid userId = GetUserId(throwExceptionIfNotAuth: true);
        CompanyOutput output = await _create.Execute(userId, input);

        return output;
    }

    [AuthorizeFilter]
    [HttpGet]
    public async Task<ActionResult<CompanyOutput?>> Get(Guid companyId)
    {
        CompanyOutput? output = await _get.Execute(companyId);
        return output;
    }

    [AuthorizeFilter]
    [HttpGet("GetAll")]
    public async Task<ActionResult<List<CompanyOutput>?>> GetAll()
    {
        List<CompanyOutput>? output = await _get.Execute();
        return output;
    }
}