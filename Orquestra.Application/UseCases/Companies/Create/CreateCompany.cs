using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.Base;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.CreateRange;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
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
        ICreateRangeCompanyUser createRangeCompanyUser,
        ICreateVerification createVerification,
        ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser,
        IEmailService emailService
    ) : CompanyBase(context, checkIfUserIsLinkedCompanyUser), ICreateCompany
{
    private readonly Context _context = context;
    private readonly IEnvService _env = env;
    private readonly ICreateVerification _createVerification = createVerification;
    private readonly ICreateRangeCompanyUser _createRangeCompanyUser = createRangeCompanyUser;
    private readonly IEmailService _emailService = emailService;

    public async Task<CompanyOutput> Execute(Guid userIdAuth, CompanyInput input)
    {
        await Validate(input, userIdAuth, isCreate: true);

        Company company = await Save(input);

        Verification verification = await SaveVerification(company);

        await SaveCompanyFirstAdministrator(userIdAuth, company);

        await SendEmail(userIdAuth, company, verification);

        var output = company.Adapt<CompanyOutput>();

        return output;
    }

    #region extras
    private async Task<Company> Save(CompanyInput input)
    {
        var company = input.Adapt<Company>();

        company.CompanySituation = CompanySituationEnum.ApprovedButNotPaid;
        company.PlanStartDate = GetDate();
        company.PlanEndDate = GetDate().AddDays(7);

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

    private async Task SaveCompanyFirstAdministrator(Guid userIdAuth, Company input)
    {
        CompanyUserInput companyUser = new()
        {
            CompanyId = input.CompanyId,
            UserId = userIdAuth,
            CompanyUserRole = CompanyUserRoleEnum.Administrator
        };

        List<CompanyUserInput> companyUsers = [companyUser];

        _ = await _createRangeCompanyUser.Execute(userIdAuth, companyUsers);
    }

    private async Task SendEmail(Guid userIdAuth, Company company, Verification verification)
    {
        var user = await _context.Users.
                   AsNoTracking().
                   Where(x => x.UserId == userIdAuth && x.Status == true).
                   FirstOrDefaultAsync() ?? throw new Exception("Sua empresa foi criada na plataforma, mas houve uma falha em disparar o e-mail de verificação porque as informações do usuário não foram encontradas.");

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