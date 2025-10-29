using Mapster;
using Orquestra.Application.UseCases.Companies.Base;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyInvoices.Create;
using Orquestra.Application.UseCases.CompanyUsers.Invite;
using Orquestra.Application.UseCases.CompanyUsers.UpdateCurrentMain;
using Orquestra.Application.UseCases.Integrations.Whatsapp.Create;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Companies.Create;

public sealed class CreateCompany(CompanyBaseDependencies deps) : CompanyBase(deps), ICreateCompany
{
    private readonly Context _context = deps.Context;
    private readonly IInviteCompanyUser _inviteCompanyUser = deps.InviteCompanyUser;
    private readonly IUpdateCurrentMainCompanyUser _updateCurrentMainCompanyUser = deps.UpdateCurrentMainCompanyUser;
    private readonly IGetUser _getUser = deps.GetUser;
    private readonly ICreateCompanyInvoice _createCompanyInvoice = deps.CreateCompanyInvoice;
    private readonly ICreateIntegrationWhatsapp _createIntegrationWhatsapp = deps.CreateIntegrationWhatsapp;

    public async Task<CompanyOutput> Execute(Guid userIdAuth, CompanyInput input)
    {
        // Validar;
        await Validate(input, userIdAuth, isCreate: true, mustValidateIfNameAlreadyExist: true, mustValidateIfEmailAlreadyExist: true);

        // Criar;
        Company company = await Save(input);
        UserOutput user = await _getUser.Execute(userId: userIdAuth, throwIfStatusFalse: true);
        await SaveCompanyFirstAdministrator(userIdAuth, company, user);

        // E-mail;
        await SendEmail(company, user);

        // Invoice;
        await _createCompanyInvoice.Execute(userIdAuth, companyId: company.CompanyId, planType: PlanTypeEnum.Free, isCreateCompany: true);

        // Integração;
        await _createIntegrationWhatsapp.Execute(userIdAuth, companyId: company.CompanyId, input: null);

        var output = company.Adapt<CompanyOutput>();

        return output;
    }

    #region extras
    private async Task<Company> Save(CompanyInput input)
    {
        var company = input.Adapt<Company>();

        (decimal _, int _, string _, string[] _, int durationDays) = PlanTypeHelper.GetValues(PlanTypeEnum.Free);

        company.PlanType = PlanTypeEnum.Free;
        company.CompanySituation = CompanySituationEnum.Approved;
        company.PlanStartDate = GetDate();
        company.PlanEndDate = GetDate().AddDays(durationDays);

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

        await _context.AddAsync(company);
        await _context.SaveChangesAsync();

        // Forçar a atualização para o false;
        company.Status = false;
        _context.Update(company);
        await _context.SaveChangesAsync();

        return company;
    }

    private async Task SaveCompanyFirstAdministrator(Guid userIdAuth, Company input, UserOutput user)
    {
        await _inviteCompanyUser.Execute(userIdAuth, companyId: input.CompanyId, email: user.Email, isFirstAdministrator: true);
        await _updateCurrentMainCompanyUser.Execute(userIdAuth, companyId: input.CompanyId);
    }
    #endregion
}