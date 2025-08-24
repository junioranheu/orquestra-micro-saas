using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Orquestra.Application.UseCases.Companies.Base;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.CreateRange;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Services.Email;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Companies.Create;

public sealed class CreateCompany(
        Context context,
        IHostEnvironment env,
        ICreateRangeCompanyUser createRangeCompanyUser,
        ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser,
        IEmailService emailService
    ) : CompanyBase(context, checkIfUserIsLinkedCompanyUser), ICreateCompany
{
    private readonly Context _context = context;
    private readonly IHostEnvironment _env = env;
    private readonly ICreateRangeCompanyUser _createRangeCompanyUser = createRangeCompanyUser;
    private readonly IEmailService _emailService = emailService;

    public async Task<CompanyOutput> Execute(Guid userIdAuth, CompanyInput input)
    {
        await Validate(input, userIdAuth, isCreate: true);

        Company company = await Save(input);

        await SaveCompanyFirstAdministrator(userIdAuth, company);

        await SendEmail(userIdAuth, company);

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
        company.IsAccountVerified = false;
        company.VerifyToken = GenerateSafeToken32Bytes(urlSafe: true);

        await _context.AddAsync(company);
        await _context.SaveChangesAsync();

        return company;
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

    private async Task SendEmail(Guid userIdAuth, Company company)
    {
        User? user = await _context.Users.
                     AsNoTracking().
                     Where(x => x.UserId == userIdAuth && x.Status == true).
                     FirstOrDefaultAsync() ?? throw new Exception("Sua empresa foi criada na plataforma, mas houve uma falha em disparar o e-mail de verificação porque as informações do usuário não foram encontradas.");

        (string urlBack, string _) = GetUrls(isProd: _env.IsProduction());
        string verifyUrl = $"{urlBack}/Company/Verify/{company.VerifyToken}";

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