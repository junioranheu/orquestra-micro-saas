using Mapster;
using Orquestra.Application.UseCases.Companies.Base;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyInvoices.Create;
using Orquestra.Application.UseCases.CompanyUsers.Invite;
using Orquestra.Application.UseCases.CompanyUsers.UpdateCurrentMain;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Application.UseCases.Verifications.Create;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Services.Email;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.Infrastructure.Services.Env.Models;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Companies.Create;

public sealed class CreateCompany(CompanyBaseDependencies deps) : CompanyBase(deps), ICreateCompany
{
    private readonly Context _context = deps.Context;
    private readonly IEnvService _env = deps.Env;
    private readonly ICreateVerification _createVerification = deps.CreateVerification;
    private readonly IInviteCompanyUser _inviteCompanyUser = deps.InviteCompanyUser;
    private readonly IUpdateCurrentMainCompanyUser _updateCurrentMainCompanyUser = deps.UpdateCurrentMainCompanyUser;
    private readonly IGetUser _getUser = deps.GetUser;
    private readonly IEmailService _emailService = deps.EmailService;
    private readonly ICreateCompanyInvoice _createCompanyInvoice = deps.CreateCompanyInvoice;

    public async Task<CompanyOutput> Execute(Guid userIdAuth, CompanyInput input)
    {
        // Validar;
        await Validate(input, userIdAuth, isCreate: true, mustValidateIfNameAlreadyExist: true, mustValidateIfEmailAlreadyExist: true);

        // Criar;
        Company company = await Save(input);
        UserOutput user = await _getUser.Execute(userId: userIdAuth, throwIfStatusFalse: true);
        await SaveCompanyFirstAdministrator(userIdAuth, company, user);

        // E-mail;
        Verification verification = await SaveVerification(company);
        await SendEmail(company, verification, user);

        // Criar cobrança;
        await _createCompanyInvoice.Execute(userIdAuth, companyId: company.CompanyId, modules: input.Modules ?? [], isCreateCompany: true);

        var output = company.Adapt<CompanyOutput>();

        return output;
    }

    #region extras
    private async Task<Company> Save(CompanyInput input)
    {
        var company = input.Adapt<Company>();

        if (input.Modules is null || input.Modules.Length == 0)
        {
            company.CompanySituation = CompanySituationEnum.RegisteredButWithoutAnyModules;
            company.PlanStartDate = null;
            company.PlanEndDate = null;
        }
        else
        {
            company.CompanySituation = CompanySituationEnum.PendingPayment;
            company.PlanStartDate = GetDate();
            company.PlanEndDate = GetDate().AddDays(SystemConsts.PlanDurationInDays);
        }

        if (input.LogoFormFile is not null)
        {
            using MemoryStream ms = new();
            await input.LogoFormFile.CopyToAsync(ms);
            company.Logo = ms.ToArray();
            company.LogoContentType = input.LogoFormFile.ContentType;
        }

        await _context.AddAsync(company);
        await _context.SaveChangesAsync();

        // Forçar a atualização para o false (burlar o Context/SaveChangesAsync);
        company.Status = false;
        _context.Update(company);
        await _context.SaveChangesAsync();

        return company;
    }

    private async Task<Verification> SaveVerification(Company input)
    {
        Verification verification = await _createVerification.Execute<Company>(entityId: input.CompanyId, verificationType: VerificationTypeEnum.Company);

        return verification;
    }

    private async Task SaveCompanyFirstAdministrator(Guid userIdAuth, Company input, UserOutput user)
    {
        await _inviteCompanyUser.Execute(userIdAuth, companyId: input.CompanyId, email: user.Email, isFirstAdministrator: true);
        await _updateCurrentMainCompanyUser.Execute(userIdAuth, companyId: input.CompanyId);
    }

    private async Task SendEmail(Company company, Verification verification, UserOutput user)
    {
        EnvOutput env = _env.GetUrls();
        string verifyUrl = $"{env.UrlBackend}/Company/Verify/{verification.Token}";

        Dictionary<string, string> values = new()
        {
            { "[NameApp]", SystemConsts.NameApp },
            { "[CompanyName]", company.Name },
            { "[UserName]", GetFirstWord(user.FullName) },
            { "[VerifyUrl]", verifyUrl }
        };

        string bodyHtml = _emailService.RenderTemplate("EmailVerifyCompany.html", values);
        await _emailService.SendEmail(to: company.Email, subject: $"Bem-vindo ao {SystemConsts.NameApp} — Verifique sua empresa!", body: bodyHtml, cc: [user.Email]);
    }
    #endregion
}