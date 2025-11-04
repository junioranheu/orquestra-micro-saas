using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Users.Base;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Application.UseCases.Verifications.Create;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Services.Email;
using Orquestra.Infrastructure.Services.Email.Models;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.Infrastructure.Services.Env.Models;
using static Orquestra.Utils.Fixtures.Encrypt;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Users.Create;

public sealed class CreateUser(
        Context context,
        IEnvService env,
        ICreateVerification createVerification,
        IEmailService emailService,
        IGetUser getUser
    ) : UserBase(getUser), ICreateUser
{
    private readonly Context _context = context;
    private readonly IEnvService _env = env;
    private readonly ICreateVerification _createVerification = createVerification;
    private readonly IEmailService _emailService = emailService;

    public async Task<UserOutput> Execute(UserInput input)
    {
        await Validate(input, userIdAuth: Guid.Empty, isCreate: true, hasChangedPassword: true);

        bool isInvite = !string.IsNullOrEmpty(input.InviteToken);
        User user = await Save(input, isInvite);

        if (!isInvite)
        {
            Verification verification = await SaveVerification(user);
            await SendEmail(user, verification);
        }
        else
        {
            await LinkUser(user, input.InviteToken!);
        }

        var output = user.Adapt<UserOutput>();

        return output;
    }

    #region extras
    private async Task<User> Save(UserInput input, bool isInvite)
    {
        if (string.IsNullOrEmpty(input.FullName) || string.IsNullOrEmpty(input.Email) || string.IsNullOrEmpty(input.Password))
        {
            throw new ArgumentException("Os dados do usuário não podem ser nulos.");
        }

        User user = new()
        {
            FullName = input.FullName,
            Email = input.Email,
            Password = EncryptPassword(input.Password),
            Role = UserRoleEnum.Common,
            RecoverPasswordQuestion = input.RecoverPasswordQuestion,
            RecoverPasswordAnswer = input.RecoverPasswordAnswer
        };

        await _context.AddAsync(user);
        await _context.SaveChangesAsync();

        // Forçar a atualização para o false caso NÃO SEJA invite (burlar o Context/SaveChangesAsync);
        if (!isInvite)
        {
            user.Status = false;
            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        return user;
    }

    private async Task<Verification> SaveVerification(User input)
    {
        Verification verification = await _createVerification.Execute<User>(entityId: input.UserId, verificationType: VerificationTypeEnum.User);

        return verification;
    }

    private async Task SendEmail(User user, Verification verification)
    {
        EnvOutput env = _env.GetUrls();
        string verifyUrl = $"{env.UrlBackend}/User/Verify/{verification.Token}";

        Dictionary<string, string> values = new()
        {
            { "[NameApp]", SystemConsts.App.NameApp },
            { "[UserName]", GetFirstWord(user.FullName) },
            { "[VerifyUrl]", verifyUrl }
        };

        string bodyHtml = _emailService.RenderTemplate(SystemConsts.Templates.EmailVerifyUser, values);

        EmailInput input = new()
        {
            To = user.Email,
            Subject = $"Bem-vindo ao {SystemConsts.App.NameApp} — Verifique sua conta!",
            Body = bodyHtml,
        };

        await _emailService.SendEmail(input);
    }

    private async Task LinkUser(User user, string inviteToken)
    {
        if (string.IsNullOrEmpty(inviteToken))
        {
            throw new ArgumentException($"O parâmetro {nameof(inviteToken)} está vazio.");
        }

        Verification? verification = await _context.Verifications.AsNoTracking().Where(x => x.Token == inviteToken).FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundVerification);

        if (string.IsNullOrEmpty(verification.Reference))
        {
            throw new ArgumentException($"O parâmetro {nameof(verification.Reference)} está vazio.");
        }

        User? inviter = await _context.Users.AsNoTracking().Where(x => x.UserId == verification.CreatedBy).FirstOrDefaultAsync();
        Guid companyId = Guid.Parse(verification.Reference);

        CompanyUser input = new()
        {
            CompanyId = companyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member,
            UserModules = [],
            IsCurrentMainCompanyUser = true,
            InviterUserId = inviter is not null ? inviter.UserId : Guid.Empty
        };

        await _context.AddAsync(input);
        await _context.SaveChangesAsync();
    }
    #endregion
}