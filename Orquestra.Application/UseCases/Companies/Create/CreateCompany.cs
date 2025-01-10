using AutoMapper;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CreateRange;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Companies.Create;

public sealed class CreateCompany(Context context, IMapper map, ICreateRangeCompanyUser createRangeCompanyUser) : ICreateCompany
{
    private readonly Context _context = context;
    private readonly IMapper _map = map;
    private readonly ICreateRangeCompanyUser _createRangeCompanyUser = createRangeCompanyUser;

    public async Task<CompanyOutput> Execute(Guid userId, CompanyInput input)
    {
        await Validations(input);
        Company company = await SaveCompany(input);
        await SaveCompanyUsers(userId, company);

        CompanyOutput? output = _map.Map<CompanyOutput>(company);

        return output;
    }

    private async Task Validations(CompanyInput input)
    {
        //(User? checkUserByUserName, string _) = await _getUserByUserNameOrEmail.Execute(input.UserName);

        //if (checkUserByUserName is not null)
        //{
        //    throw new Exception("Já existe um usuário com esse nome de usuário");
        //}

        //(User? checkUserByEmail, string _) = await _getUserByUserNameOrEmail.Execute(input.Email);

        //if (checkUserByEmail is not null)
        //{
        //    throw new Exception("Já existe um usuário com esse e-mail");
        //}
    }

    private async Task<Company> SaveCompany(CompanyInput input)
    {
        DateTime date = GetDate();

        Company company = _map.Map<Company>(input);
        company.Status = true;
        company.CreatedDate = date;

        await _context.AddAsync(company);
        await _context.SaveChangesAsync();

        return company;
    }

    private async Task SaveCompanyUsers(Guid userId, Company input)
    {
        CompanyUser companyUser = new()
        {
            CompanyUserId = input.CompanyId,
            UserId = userId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator,
            CreatedDate = GetDate()
        };

        List<CompanyUser> companyUsers = [companyUser];

        await _createRangeCompanyUser.Execute(companyUsers);
    }
}