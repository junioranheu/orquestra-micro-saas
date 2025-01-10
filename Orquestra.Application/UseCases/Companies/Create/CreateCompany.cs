using AutoMapper;
using Orquestra.Application.UseCases.Companies.Base;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CreateRange;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Companies.Create;

public sealed class CreateCompany(Context context, IMapper map, ICreateRangeCompanyUser createRangeCompanyUser) : CompanyBase(context), ICreateCompany
{
    private readonly Context _context = context;
    private readonly IMapper _map = map;
    private readonly ICreateRangeCompanyUser _createRangeCompanyUser = createRangeCompanyUser;

    public async Task<CompanyOutput> Execute(Guid userId, CompanyInput input)
    {
        await Validate(input);
        Company company = await SaveCompany(input);
        await SaveCompanyUsers(userId, company);

        CompanyOutput? output = _map.Map<CompanyOutput>(company);

        return output;
    }

    private async Task<Company> SaveCompany(CompanyInput input)
    {
        Company company = _map.Map<Company>(input);

        company.CompanySituation = CompanySituationEnum.ApprovedButNotPaid;
        company.PlanStartDate = GetDate();
        company.PlanEndDate = GetDate().AddMonths(1);

        await _context.AddAsync(company);
        await _context.SaveChangesAsync();

        return company;
    }

    private async Task SaveCompanyUsers(Guid userId, Company input)
    {
        CompanyUser companyUser = new()
        {
            CompanyId = input.CompanyId,
            UserId = userId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator
        };

        List<CompanyUser> companyUsers = [companyUser];

        await _createRangeCompanyUser.Execute(companyUsers);
    }
}