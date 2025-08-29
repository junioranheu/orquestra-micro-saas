using Mapster;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.Base;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
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

namespace Orquestra.Application.UseCases.CompanyUsers.Invite;

public sealed class InviteCompanyUser(
        Context context,
        IEnvService env,
        ICreateVerification createVerification,
        ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser,
        IGetUser getUser,
        IGetCompany getCompany,
        IEmailService emailService
    ) : CompanyUserBase(context, checkIfUserIsLinkedCompanyUser), IInviteCompanyUser
{
    private readonly Context _context = context;
    private readonly ICreateVerification _createVerification = createVerification;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;
    private readonly IEnvService _env = env;
    private readonly IGetUser _getUser = getUser;
    private readonly IGetCompany _getCompany = getCompany;
    private readonly IEmailService _emailService = emailService;

    public async Task Execute(Guid userIdAuth, Guid companyId, string email)
    {
        // Validar;
        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentException("O e-mail não pode ser vazio.");
        }

        email = GetNormalizedLowerStr(email);

        CompanyOutput company = await _getCompany.Execute(userIdAuth: userIdAuth, companyId: companyId);
        UserOutput user = await _getUser.Execute(userId: Guid.Empty, email: email, throwIfStatusFalse: false);

        if (user is null)
        {
            await InviteUserWhoDoesntHaveAccount(userIdAuth, company, email);
            return;
        }

        await InviteUserWhoAlreadyHaveAccount(userIdAuth, company, user);
    }

    #region extras
    private async Task InviteUserWhoDoesntHaveAccount(Guid userIdAuth, CompanyOutput company, string email)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: company.CompanyId, userId: userIdAuth, needCompanyAdmin: true);

        Verification verification = await SaveVerification(companyUserId: Guid.Empty);
        UserOutput user = new() { Email = email };

        await SendEmail(company, user, companyUserRole: CompanyUserRoleEnum.Member, verification);
    }

    private async Task InviteUserWhoAlreadyHaveAccount(Guid userIdAuth, CompanyOutput company, UserOutput user)
    {
        CompanyUserInput input = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Validate(input: input, userIdAuth, isCreate: true);

        var companyUser = input.Adapt<CompanyUser>();

        // Normalizar dados;
        companyUser.InviterUserId = userIdAuth;

        // Salvar;
        await _context.AddAsync(companyUser);
        await _context.SaveChangesAsync();

        Verification verification = await SaveVerification(companyUserId: companyUser.CompanyUserId);
        await SendEmail(company, user, companyUserRole: companyUser.CompanyUserRole, verification);
    }

    private async Task<Verification> SaveVerification(Guid companyUserId)
    {
        Verification verification = await _createVerification.Execute<CompanyUser>(entityId: companyUserId, verificationType: VerificationTypeEnum.CompanyUser);

        return verification;
    }

    private async Task SendEmail(CompanyOutput company, UserOutput user, CompanyUserRoleEnum companyUserRole, Verification verification)
    {
        EnvOutput env = _env.GetUrls();
        string verifyUrl = $"{env.UrlBackend}/CompanyUser/Verify/{verification.Token}";

        Dictionary<string, string> values = new()
        {
            { "[NameApp]", SystemConsts.NameApp },
            { "[CompanyName]", company.Name },
            { "[UserName]", GetFirstWord(user.FullName) ?? user.Email },
            { "[CompanyUserRole]", GetEnumDesc(companyUserRole).ToLowerInvariant() },
            { "[VerifyUrl]", verifyUrl },
        };

        string bodyHtml = _emailService.RenderTemplate("EmailVerifyCompanyUser.html", values);
        await _emailService.SendEmail(to: user.Email, subject: $"{company.Name} - Bem-vindo ao {SystemConsts.NameApp} — Verifique sua conta!", body: bodyHtml);
    }
    #endregion
}