using Mapster;
using Orquestra.Application.UseCases.Users.Base;
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
        await Validate(input, userIdAuth: Guid.Empty, isCreate: true);

        User user = await Save(input);

        Verification verification = await SaveVerification(user);

        await SendEmail(user, verification);

        var output = user.Adapt<UserOutput>(); 

        return output;
    }

    #region extras
    private async Task<User> Save(UserInput input)
    {
        if (string.IsNullOrEmpty(input.FullName) || string.IsNullOrEmpty(input.Email) || string.IsNullOrEmpty(input.Password))
        {
            throw new Exception("Os dados do usuário não podem ser nulos.");
        }

        User user = new()
        {
            FullName = input.FullName,
            Email = input.Email,
            Password = EncryptPassword(input.Password),
            Role = UserRoleEnum.Common
        };

        await _context.AddAsync(user);
        await _context.SaveChangesAsync();

        // Forçar a atualização para o false (burlar o Context/SaveChangesAsync);
        user.Status = false;
        _context.Update(user);
        await _context.SaveChangesAsync();

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
            { "[NameApp]", SystemConsts.NameApp },
            { "[UserName]", GetFirstWord(user.FullName) },
            { "[VerifyUrl]", verifyUrl }
        };

        string bodyHtml = _emailService.RenderTemplate("EmailVerifyUser.html", values);
        await _emailService.SendEmail(to: user.Email, subject: $"Bem-vindo ao {SystemConsts.NameApp} — Verifique sua conta!", body: bodyHtml);
    }
    #endregion
}