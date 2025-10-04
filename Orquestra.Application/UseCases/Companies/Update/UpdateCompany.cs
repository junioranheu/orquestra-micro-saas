using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.Base;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Companies.Update;

public sealed class UpdateCompany(CompanyBaseDependencies deps) : CompanyBase(deps), IUpdateCompany
{
    private readonly Context _context = deps.Context;

    public async Task<CompanyOutput> Execute(Guid userIdAuth, CompanyInput input)
    {
        Company? company = await _context.Companies.
                           // AsNoTracking(). // Propositalmente sem AsNoTracking;
                           Where(x => x.CompanyId == input.CompanyId).
                           FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warn_NotFound_Company);

        bool mustValidateIfNameAlreadyExist = company.Name != input.Name;
        bool mustValidateIfEmailAlreadyExist = company.Email != input.Email;

        await Validate(input, userIdAuth, isCreate: false, mustValidateIfNameAlreadyExist, mustValidateIfEmailAlreadyExist);
        await Update(input, company);

        var output = company.Adapt<CompanyOutput>();

        return output;
    }

    #region extras
    private async Task<Company> Update(CompanyInput input, Company company)
    {
        company.Name = input.Name;
        company.Email = input.Email;
        company.Phone = input.Phone;
        company.CompanyType = input.CompanyType;
        company.Address = input.Address;
        company.City = input.City;
        company.State = input.State;
        company.ZipCode = input.ZipCode;
        company.Country = input.Country;
        company.Color = input.Color;

        if (input.LogoFormFile is not null)
        {
            using MemoryStream ms = new();
            await input.LogoFormFile.CopyToAsync(ms);
            company.Logo = ms.ToArray();
            company.LogoContentType = input.LogoFormFile.ContentType;
        }
        else
        {
            company.Logo = null;
            company.LogoContentType = null;
        }

        _context.Update(company);
        await _context.SaveChangesAsync();

        return company;
    }
    #endregion
}