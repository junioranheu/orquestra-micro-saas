using Microsoft.AspNetCore.Mvc;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Companies.Create;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Domain.Enums;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController(ICreateCompany create, IGetCompany get) : BaseController<CompanyController>
{
    private readonly ICreateCompany _create = create;
    private readonly IGetCompany _get = get;

    [AuthorizeFilter]
    [HttpPost]
    public async Task<ActionResult> Create([FromForm] CompanyInput input)
    {
        Guid userId = GetUserId(throwExceptionIfNotAuth: true);
        CompanyOutput output = await _create.Execute(userId, input);
        // TO DO: Enviar e-mail para o owner (especialmente para dar boas-vindas e falar sobre o plano);

        return Ok(output);
    }

    [AuthorizeFilter]
    [HttpGet]
    public async Task<ActionResult> Get(Guid companyId)
    {
        Guid userId = GetUserId(throwExceptionIfNotAuth: true);
        CompanyOutput? output = await _get.Execute(userId, companyId);
        return Ok(output);
    }

    [AuthorizeFilter(UserRoleEnum.Admin, UserRoleEnum.Maintainer)]
    [HttpGet("GetAll")]
    public async Task<ActionResult> GetAll()
    {
        List<CompanyOutput>? output = await _get.Execute();
        return Ok(output);
    }
}