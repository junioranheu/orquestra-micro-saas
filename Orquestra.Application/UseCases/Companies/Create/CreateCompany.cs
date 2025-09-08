using Mapster;
using Orquestra.Application.UseCases.Companies.Base;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyInvoices.Create;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
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

public sealed class CreateCompany(
        Context context,
        IEnvService env,
        ICreateVerification createVerification,
        IInviteCompanyUser inviteCompanyUser,
        IUpdateCurrentMainCompanyUser updateCurrentMainCompanyUser,
        IGetUser getUser,
        IEmailService emailService,
        ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser,
        ICreateCompanyInvoice createCompanyInvoice
    ) : CompanyBase(context, checkIfUserIsLinkedCompanyUser), ICreateCompany
{
    private readonly Context _context = context;
    private readonly IEnvService _env = env;
    private readonly ICreateVerification _createVerification = createVerification;
    private readonly IInviteCompanyUser _inviteCompanyUser = inviteCompanyUser;
    private readonly IUpdateCurrentMainCompanyUser _updateCurrentMainCompanyUser = updateCurrentMainCompanyUser;
    private readonly IGetUser _getUser = getUser;
    private readonly IEmailService _emailService = emailService;
    private readonly ICreateCompanyInvoice _createCompanyInvoice = createCompanyInvoice;

    public async Task<CompanyOutput> Execute(Guid userIdAuth, CompanyInput input)
    {
        // Validar;
        await Validate(input, userIdAuth, isCreate: true);

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

        company.CompanySituation = CompanySituationEnum.RegisteredButWithoutAnyModules;

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