using Mapster;
using Microsoft.EntityFrameworkCore;
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

    public async Task Execute(Guid userIdAuth, Guid companyId, string email, bool isFirstAdministrator)
    {
        #region validations
        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentException("O e-mail não pode ser vazio.");
        }

        bool checkEmail = IsEmailValid(email);

        if (!checkEmail)
        {
            throw new ArgumentException("O e-mail não é válido.");
        }

        email = GetNormalizedLowerStr(email);
        #endregion

        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: true);

        CompanyOutput company = await _getCompany.Execute(userIdAuth: userIdAuth, companyId: companyId, throwIfStatusFalse: !isFirstAdministrator);
        UserOutput user = await _getUser.Execute(userId: Guid.Empty, email: email, throwIfStatusFalse: false);

        if (!isFirstAdministrator && (user is null || user.UserId == Guid.Empty))
        {
            await InviteUserWhoDoesntHaveAccount(company, email);
            return;
        }

        await InviteUserWhoAlreadyHaveAccount(userIdAuth, company, user, isFirstAdministrator);
    }

    #region extras
    private async Task InviteUserWhoDoesntHaveAccount(CompanyOutput company, string email)
    {
        Verification verification = await SaveVerification(companyUserId: Guid.Empty, companyId: company.CompanyId);
        UserOutput user = new() { Email = email };

        await SendEmail(company, user, companyUserRole: CompanyUserRoleEnum.Member, verification);
    }

    private async Task InviteUserWhoAlreadyHaveAccount(Guid userIdAuth, CompanyOutput company, UserOutput user, bool isFirstAdministrator)
    {
        bool alreadyIsMember = await _context.CompanyUsers.
                               Include(x => x.User).
                               AsNoTracking().
                               AnyAsync(x =>
                                  x.CompanyId == company.CompanyId &&
                                  x.User!.Email.ToLower() == user.Email.ToLower() &&
                                  x.Status == true
                               );

        if (alreadyIsMember)
        {
            throw new InvalidOperationException($"Este e-mail já faz parte dos membros desta empresa.");
        }

        CompanyUserInput input = new()
        {
            CompanyId = company.CompanyId,
            UserId = isFirstAdministrator ? userIdAuth : user.UserId,
            CompanyUserRole = isFirstAdministrator ? CompanyUserRoleEnum.Administrator : CompanyUserRoleEnum.Member,
            UserModules = []
        };

        await Validate(input: input, userIdAuth, isCreate: true);

        var companyUser = input.Adapt<CompanyUser>();

        // Normalizar dados;
        companyUser.InviterUserId = isFirstAdministrator ? null : userIdAuth;
        companyUser.IsCurrentMainCompanyUser = isFirstAdministrator;


        // Salvar;
        await _context.AddAsync(companyUser);
        await _context.SaveChangesAsync();

        companyUser.Status = false;
        _context.Update(companyUser);
        await _context.SaveChangesAsync();

        if (isFirstAdministrator)
        {
            return;
        }

        Verification verification = await SaveVerification(companyUserId: companyUser.CompanyUserId, companyId: company.CompanyId);
        await SendEmail(company, user, companyUserRole: companyUser.CompanyUserRole, verification);
    }

    private async Task<Verification> SaveVerification(Guid companyUserId, Guid companyId)
    {
        Verification verification = await _createVerification.Execute<CompanyUser>(entityId: companyUserId, verificationType: VerificationTypeEnum.CompanyUser, reference: companyId.ToString());

        return verification;
    }

    private async Task SendEmail(CompanyOutput company, UserOutput user, CompanyUserRoleEnum companyUserRole, Verification verification)
    {
        bool userHasAccount = !string.IsNullOrWhiteSpace(user.FullName);

        EnvOutput env = _env.GetUrls();
        string verifyUrl = userHasAccount ? $"{env.UrlBackend}/CompanyUser/Verify/{verification.Token}" : $"{env.UrlFrontend}/criar-conta?token={verification.Token}";

        Dictionary<string, string> values = new()
        {
            { "[NameApp]", SystemConsts.App.NameApp },
            { "[CompanyName]", company.Name },
            { "[UserName]", userHasAccount ? GetFirstWord(user.FullName) : user.Email },
            { "[CompanyUserRole]", GetEnumDesc(companyUserRole).ToLowerInvariant() },
            { "[VerifyUrl]", verifyUrl },
        };

        string bodyHtml = _emailService.RenderTemplate(SystemConsts.Templates.EmailVerifyCompanyUser, values);
        await _emailService.SendEmail(to: user.Email, subject: $"{company.Name} — Bem-vindo ao {SystemConsts.App.NameApp} — Verifique sua conta!", body: bodyHtml);
    }
    #endregion
}